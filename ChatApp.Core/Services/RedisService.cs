using ChatApp.Core.Interfaces;
using StackExchange.Redis;

namespace ChatApp.Core.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _redisDatabase;

        public RedisService(IConnectionMultiplexer redisDatabase)
        {
            _redisDatabase = redisDatabase.GetDatabase();
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            return await _redisDatabase.KeyExistsAsync(key);
        }

        public async Task SetValueAsync(string key, string value, TimeSpan? expire = null)
        {
            await _redisDatabase.StringSetAsync(key, value, expire);
        }

        public async Task<string?> GetValueAsync(string key)
        {
            return await _redisDatabase.StringGetAsync(key);
        }

        public async Task DeleteKeyAsync(string key)
        {
            await _redisDatabase.KeyDeleteAsync(key);
        }
    }
}
