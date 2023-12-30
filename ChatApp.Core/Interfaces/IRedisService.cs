namespace ChatApp.Core.Interfaces
{
    public interface IRedisService
    {
        Task DeleteKeyAsync(string key);
        Task<string?> GetValueAsync(string key);
        Task<bool> KeyExistsAsync(string key);
        Task SetValueAsync(string key, string value, TimeSpan? expire = null);
    }
}
