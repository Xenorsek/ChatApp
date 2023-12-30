using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.Models
{
    public class UserInfo
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public List<string> UserRoles { get; set; } = new List<string>();
    }
}
