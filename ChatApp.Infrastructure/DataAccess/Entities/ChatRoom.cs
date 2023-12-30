using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Infrastructure.DataAccess.Entities
{
    public class ChatRoom
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(150)]
        [Required]
        public required string Name { get; set; }
        public int UsersLimit { get; set; } = 10;
        public bool CreatedAsDefault { get; set; } = false;

        // Relacje z innymi encjami
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<UserChatRoom> UserChatRooms { get; set; } = new List<UserChatRoom>();
    }

}
