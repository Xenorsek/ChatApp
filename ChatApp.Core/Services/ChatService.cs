using ChatApp.Core.Models;
using ChatApp.Infrastructure.DataAccess.Entities;
using ChatApp.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Services
{
    public class ChatService
    {
        private readonly MessageRepository _messageRepository;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ChatRoomRepository _chatRoomRepository;

        public ChatService(MessageRepository messageRepository, IIdentityUserRepository identityUserRepository, ChatRoomRepository chatRoomRepository)
        {
            _messageRepository = messageRepository;
            _identityUserRepository = identityUserRepository;            _chatRoomRepository = chatRoomRepository;

        }

        public async Task AssignUserToRoom(string userIdentity, int roomId)
        {
            var isExist = await _chatRoomRepository.CheckIfRoomExists(roomId);
            if (!isExist)
            {
                throw new Exception("Chat room is not exists");
            }
            await _chatRoomRepository.AssignUserToRoom(userIdentity, roomId);
        }

        public async Task DeAttachUserFromRooms(string userIdentity)
        {
            await _chatRoomRepository.FreeUserFromRoom(userIdentity);
        }

        public async Task<List<Message>> GetMessages(string userIdentity, int roomId)
        {
            //only user in room can see messages
            var isUserAssignedToRoom = await _identityUserRepository.CheckIfUserIsInRoom(userIdentity, roomId);
            if (!isUserAssignedToRoom)
            {
                throw new Exception("User is not attached to this room");
            }

            return await _messageRepository.GetMessages(roomId);
        }

        public async Task<MessageDto> CreateMessage(string userIdentity, CreateMessageRequest messageModel, string userName = "")
        {
            //TODO: to powinno być cachowane
            var isUserAssignedToRoom = await _identityUserRepository.CheckIfUserIsInRoom(userIdentity, messageModel.RoomId);
            if (!isUserAssignedToRoom)
            {
                throw new Exception("User is not attached to this room");
            }

            //TODO: nowe wiadomosci powinny byc zapisywane również na redis do szybszego dostępu
            var result = await _messageRepository.CreateMessage(messageModel.RoomId, userIdentity, messageModel.Content);
            return new MessageDto
            {
                UserName = userName,
                CreatedAt = result.Timestamp,
                Content = messageModel.Content
            };
        }
    }
}
