using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Common.Enums
{
    public class CacheKeys
    {
        public static string ChatRoomExists(int roomId) => $"ChatRoom:{roomId}";
        public static string UserInRoom(string userId) => $"User:{userId}";

    }
}
