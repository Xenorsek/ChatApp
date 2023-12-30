using ChatApp.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Services
{
    public class HangfireService
    {
        private readonly ChatDbContext _context;

        public HangfireService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task CleaningMessagesAndCustomRooms()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE MESSAGES");
            var chatRooms = await _context.ChatRooms.Where(x => !x.CreatedAsDefault).ToListAsync();
            _context.ChatRooms.RemoveRange(chatRooms);
            await _context.SaveChangesAsync();
        }
    }
}
