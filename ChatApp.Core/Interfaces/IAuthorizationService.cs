using ChatApp.Infrastructure.Models;

namespace ChatApp.Core.Interfaces
{
    public interface IAuthorizationService
    {
        Task<UserInfo> Login(string userProvider, string password);
        Task<bool> Register(string username, string email, string password);
    }
}