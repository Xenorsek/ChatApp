using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Infrastructure.DataAccess.Entities
{
    public class UserChatRoom
    {
        public string UserId { get; set; }
        public int ChatRoomId { get; set; }
        
        public virtual User User { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }
    }
}
