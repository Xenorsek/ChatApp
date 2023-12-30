using ChatApp.Common.Enums;
using ChatApp.Infrastructure.DataAccess;
using ChatApp.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRoomRepository
    {
        private readonly ChatDbContext _context;
        private readonly IMemoryCache _cache;

        public ChatRoomRepository(ChatDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<string>> GetUsersFromRoom(int roomId)
        {
            var room = await _context.ChatRooms
                .Include(x => x.UserChatRooms)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == roomId);

            if (room == null)
            {
                return [];
            }

            var usersInRoomList = room.UserChatRooms.Select(x => x.User.UserName).ToList();
            return usersInRoomList;
        }

        public async Task<ChatRoom> CreateRoom(string name, int usersLimit)
        {
            ChatRoom chatRoom = new() { Name = name, UsersLimit = usersLimit };
            await _context.ChatRooms.AddAsync(chatRoom);
            await _context.SaveChangesAsync();

            var cacheKey = CacheKeys.ChatRoomExists(chatRoom.Id);
            _cache.Set(cacheKey, true);

            return chatRoom;
        }

        public async Task<bool> CheckIfRoomExists(int roomId)
        {
            bool isExists;
            var cacheKey = CacheKeys.ChatRoomExists(roomId);
            var isCached = _cache.TryGetValue(cacheKey, out isExists);
            if (isCached)
            {
                return isExists;
            }

            isExists =  await _context.ChatRooms.AnyAsync(x => x.Id == roomId);
            _cache.Set(cacheKey, isExists);

            return isExists;
        }

        public async Task FreeUserFromRoom(string userId)
        {
            var cacheKey = CacheKeys.UserInRoom(userId);
            List<UserChatRoom> usersRooms = new();
            bool isCached = _cache.TryGetValue(cacheKey, out usersRooms);
            if (!isCached)
            {
                usersRooms = await _context.UserChatRooms.Where(x => x.UserId == userId).ToListAsync();
            }
            
            _context.UserChatRooms.RemoveRange(usersRooms);
            _cache.Remove(cacheKey);

            await _context.SaveChangesAsync();
        }

        public async Task AssignUserToRoom(string userId, int roomId)
        {
            var cacheKey = CacheKeys.UserInRoom(userId);
            List<UserChatRoom> usersRooms = new();
            bool isCached = _cache.TryGetValue(cacheKey, out usersRooms);
            if (!isCached)
            {
                usersRooms = await _context.UserChatRooms.Where(x => x.UserId == userId).ToListAsync();
            }
            if (usersRooms.Any())
            {
                _context.UserChatRooms.RemoveRange(usersRooms);
            }

            await _context.UserChatRooms.AddAsync(new UserChatRoom { UserId = userId, ChatRoomId = roomId });
            _cache.Set(cacheKey, roomId);

            await _context.SaveChangesAsync();
        }
    }
}
