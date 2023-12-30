using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Models
{
    public class CreateMessageRequest
    {
        public required int RoomId { get; set; }
        public required string Content { get; set; }

    }
}
