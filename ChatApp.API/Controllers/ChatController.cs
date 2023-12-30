using ChatApp.API.Hubs;
using ChatApp.Core.Models;
using ChatApp.Core.Services;
using ChatApp.Infrastructure.DataAccess;
using ChatApp.Infrastructure.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _context;
        private readonly ChatService _chatService;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatController(ChatDbContext context, ChatService chatService, IHubContext<ChatHub> chatHub)
        {
            _context = context;
            _chatService = chatService;
            _chatHub = chatHub;
        }


        [HttpGet("GetMessages/{roomId}")]
        public async Task<IActionResult> GetMessages(int roomId)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _chatService.GetMessages(user, roomId);
            return Ok(result);
        }

        [HttpPost("CreateMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageRequest model)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = await _chatService.CreateMessage(user, model);

            await _chatHub.Clients.Group(model.RoomId.ToString()).SendAsync("ReceiveMessage", new {username = "abc", message = "abc"});
            return Ok();
        }

        [Authorize]
        [HttpGet("chat")]
        public IEnumerable<ChatRoom> Get()
        {
            return _context.ChatRooms.ToList();
        }

        [Authorize("AdminOnly")]
        [HttpGet("admin")]
        public IEnumerable<ChatRoom> GetAdmin()
        {
            return _context.ChatRooms.ToList();
        }

        [HttpGet("anonymous")]
        public IEnumerable<ChatRoom> GetAnonymous()
        {
            var a = User;
            return _context.ChatRooms.ToList();
        }
    }
}
