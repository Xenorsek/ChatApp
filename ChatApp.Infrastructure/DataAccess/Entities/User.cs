using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.DataAccess.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<UserChatRoom> UserChatRooms { get; set; }
        
    }
}
