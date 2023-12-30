using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Infrastructure.DataAccess.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(500)]
        public required string Content { get; set; }
        public DateTime Timestamp { get; set; }

        // Klucze obce
        [Required]
        public required string UserId { get; set; }
        [Required]
        public int ChatRoomId { get; set; }

        // Nawigacja do innych encji
        public virtual IdentityUser User { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }
    }

}
