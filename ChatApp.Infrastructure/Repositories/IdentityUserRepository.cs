using ChatApp.Common.Enums;
using ChatApp.Infrastructure.DataAccess;
using ChatApp.Infrastructure.DataAccess.Entities;
using ChatApp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class IdentityUserRepository : IIdentityUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ChatDbContext _context;

        public IdentityUserRepository(UserManager<User> userManager, SignInManager<User> signInManager, ChatDbContext chatDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = chatDbContext;
        }

        public async Task<bool> RegisterUser(string username, string email, string password)
        {
            bool isValidUserName = !await _userManager.Users.AnyAsync(x => (x.UserName != null ? x.UserName.ToLower() : string.Empty) == username.ToLower());
            if (!isValidUserName)
            {
                throw new ArgumentException("UserName already taken");
            }

            bool isValidEmail = !await _userManager.Users.AnyAsync(x => (x.Email != null ? x.Email.ToLower() : string.Empty) == email.ToLower());
            if (!isValidEmail)
            {
                throw new ArgumentException("Account with that email address already exists");
            }

            var user = new User { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            await _userManager.AddToRoleAsync(user, Roles.User.ToString());

            return result.Succeeded;
        }

        public async Task<IList<string>> GetUserRoles(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> CheckIfUserIsInRoom(string userId, int roomId)
        {
            var isAssignedToRoom = await _context.UserChatRooms
                .AnyAsync(uc => uc.UserId == userId && uc.ChatRoomId == roomId);
            return isAssignedToRoom;
        }

        public async Task<IList<ChatRoom>> GetUsersChatRooms(string userId)
        {
            var userRooms = await _context.UserChatRooms
                .Where(uc => uc.UserId == userId)
                .Include(uc => uc.ChatRoom)
                .Select(uc => uc.ChatRoom)
                .ToListAsync();

            return userRooms;
        } 

        public async Task<UserInfo?> LoginUser(string loginProvider, string password)
        {
            User? user;
            if (loginProvider.Contains('@'))
            {
                user = await _userManager.FindByEmailAsync(loginProvider);
            }
            else
            {
                user = await _userManager.FindByNameAsync(loginProvider);
            }
            
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: true, lockoutOnFailure: false);
                var roles = await GetUserRoles(user);
                if (result.Succeeded)
                {
                    UserInfo userInfo = new()
                    {
                        Id = user.Id,
                        UserName = user.UserName ?? string.Empty,
                        UserRoles = (List<string>)roles
                    };
                    return userInfo;
                }
            }

            return null;
            
        }
    }
}
