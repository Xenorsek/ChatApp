using ChatApp.Core.Models;
using ChatApp.Core.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChatApp.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        // Metoda do dołączania do pokoju chatu
        public async Task JoinRoom(string roomId)
        {
            var user = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _chatService.AssignUserToRoom(user, int.Parse(roomId));

            //przylacz usera do pokoju
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserJoined", Context.ConnectionId, roomId);
        }

        // Metoda do opuszczania pokoju chatu
        public async Task LeaveRoom(string roomId)
        {
            var user = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _chatService.DeAttachUserFromRooms(user);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId, roomId);
        }

        // Metoda do wysyłania wiadomości do pokoju chatu
        public async Task SendMessageToRoom(CreateMessageRequest message)
        {
            var user = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User.Identity.Name;
            var messageDto = await _chatService.CreateMessage(user, message, userName);

            await Clients.Group(message.RoomId.ToString()).SendAsync("ReceiveMessage", Context.ConnectionId, messageDto);
        }

        // Wywoływana, gdy klient się łączy
        public override async Task OnConnectedAsync()
        {
            // Możesz tu zaimplementować logikę, która będzie wykonywana podczas łączenia klienta
            await base.OnConnectedAsync();
        }

        // Wywoływana, gdy klient się rozłącza
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _chatService.DeAttachUserFromRooms(user);

            // Możesz tu zaimplementować logikę, która będzie wykonywana podczas rozłączania klienta
            await base.OnDisconnectedAsync(exception);

        }
    }


}
