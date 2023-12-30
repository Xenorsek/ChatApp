using ChatApp.Infrastructure.DataAccess;
using ChatApp.Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class MessageRepository
    {
        private readonly ChatDbContext _context;
        
        public MessageRepository(ChatDbContext context) { 
            _context = context;
        }

        public async Task<List<Message>> GetMessages(int roomId, int timeRangeInHours = 5, int limit = 30)
        {
            var messageTimeRange = DateTime.UtcNow.AddHours(-timeRangeInHours);
            List<Message> messages = await _context.Messages
                .Where(x => x.ChatRoomId == roomId && x.Timestamp > messageTimeRange)
                .OrderByDescending(x => x.Timestamp).Take(limit).Reverse().ToListAsync();

            return messages;
        }

        public async Task<Message> CreateMessage(int roomId, string userId, string content)
        {
            var message = new Message
            {
                ChatRoomId = roomId,
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                Content = content
            };
            await _context.Messages.AddAsync(message);
            var result = await _context.SaveChangesAsync();

            return message;
        }
    }
}
