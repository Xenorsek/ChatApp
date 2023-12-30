using ChatApp.API.Middlewares;
using ChatApp.Core.Interfaces;
using ChatApp.Core.Services;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Services;
using StackExchange.Redis;

namespace ChatApp.API.Helpers
{
    public static class DependencyInjectionHelper
    {
        public static void DependencyInjection(this WebApplicationBuilder builder)
        {
            //builder.Services.AddScoped<IRedisService, RedisService>();
            builder.Services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();
            builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
            builder.Services.AddScoped<HangfireService>();
            builder.Services.AddScoped<ChatRoomRepository>();
            builder.Services.AddScoped<ErrorHandlingMiddleware>();
            builder.Services.AddScoped<MessageRepository>();
            builder.Services.AddScoped<ChatService>();

            try
            {
                //var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
                //builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis server came with error {ex}");
            }
        }
    }
}
