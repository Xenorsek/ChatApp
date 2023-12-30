using System.ComponentModel.DataAnnotations;

namespace ChatApp.Core
{
    public class LoginUserRequest
    {
        [MaxLength(150)]
        public required string LoginProvider { get; set; }
        [MaxLength(50)]
        public required string Password { get; set; }
    }
}