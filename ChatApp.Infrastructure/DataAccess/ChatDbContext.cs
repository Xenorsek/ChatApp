using ChatApp.Infrastructure.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.DataAccess
{
    public class ChatDbContext : IdentityDbContext
    {
        public ChatDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<UserChatRoom> UserChatRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserChatRoom>()
                .HasKey(uc => new { uc.UserId, uc.ChatRoomId });

            modelBuilder.Entity<UserChatRoom>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserChatRooms)
                .HasForeignKey(uc => uc.UserId);

            modelBuilder.Entity<UserChatRoom>()
                .HasOne(uc => uc.ChatRoom)
                .WithMany(c => c.UserChatRooms)
                .HasForeignKey(uc => uc.ChatRoomId);

            // Seed danych dla ChatRoom
            modelBuilder.Entity<ChatRoom>().HasData(
                new ChatRoom { Id = 1, Name = "Room1", CreatedAsDefault = true },
                new ChatRoom { Id = 2, Name = "Room2", CreatedAsDefault = true }
            );
        }
    }

}
