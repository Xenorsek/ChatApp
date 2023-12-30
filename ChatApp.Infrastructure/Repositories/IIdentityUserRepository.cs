using ChatApp.Infrastructure.DataAccess.Entities;
using ChatApp.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Infrastructure.Repositories
{
    public interface IIdentityUserRepository
    {
        Task<bool> CheckIfUserIsInRoom(string userId, int roomId);
        Task<IList<ChatRoom>> GetUsersChatRooms(string userId);
        Task<IList<string>> GetUserRoles(User user);
        Task<UserInfo?> LoginUser(string loginProvider, string password);
        Task<bool> RegisterUser(string username, string email, string password);
    }
}