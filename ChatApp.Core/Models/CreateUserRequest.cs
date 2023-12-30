using ChatApp.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Core
{
    public class CreateUserRequest
    {
        [MaxLength(150)]
        [EmailAddress]
        public required string Email { get; set; }
        [MaxLength(100)]
        [MinLength(6, ErrorMessage = "Username must be at least 6 characters long.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username can only contain letters and numbers.")]
        public required string UserName { get; set; }
        [MaxLength(50)]
        [StrongPassword]
        public required string Password { get; set; }
    }
}