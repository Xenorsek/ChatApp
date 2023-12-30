using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
namespace ChatApp.API.Helpers
{
    //https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-7.0
    public static class RateLimiterHelper
    {
        public static void AddRateLimiterHelper(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(options =>
            {
                options.OnRejected = (context, _) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

                    return new ValueTask();
                };

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
                {
                    //Jeżeli jest adres lokalny to wtedy nie ma limitera
                    IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;
                    if (!IPAddress.IsLoopback(remoteIpAddress!))
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(remoteIpAddress!, _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100, // Limit zezwoleń
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0, // Bez kolejki
                            Window = TimeSpan.FromMinutes(1) // Okno czasowe 1 minuta
                        });
                    }
                    return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
                });

                // Polityka dla logowania
                options.AddSlidingWindowLimiter("LoginLimiter", rateLimitPolicyBuilder =>
                {
                    rateLimitPolicyBuilder.PermitLimit = 5; // Limit 5 prób logowania
                    rateLimitPolicyBuilder.SegmentsPerWindow = 5;
                    rateLimitPolicyBuilder.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    rateLimitPolicyBuilder.QueueLimit = 0; // Bez kolejki
                    rateLimitPolicyBuilder.Window = TimeSpan.FromMinutes(1); // Okno czasowe 1 minuta
                });

                // Polityka dla wysyłania wiadomości
                options.AddTokenBucketLimiter("MessageLimiter", rateLimitPolicyBuilder =>
                {
                    rateLimitPolicyBuilder.TokenLimit = 1; // 1 token na sekundę
                    rateLimitPolicyBuilder.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    rateLimitPolicyBuilder.QueueLimit = 0; // Bez kolejki
                    rateLimitPolicyBuilder.ReplenishmentPeriod = TimeSpan.FromSeconds(1); // Czas odnowienia tokena
                    rateLimitPolicyBuilder.TokensPerPeriod = 1; // Ilość tokenów na okres
                });

                // Polityka dla tworzenia pokoi czatowych
                options.AddTokenBucketLimiter("ChatRoomCreationLimiter", rateLimitPolicyBuilder =>
                {
                    rateLimitPolicyBuilder.TokenLimit = 1; // Limit 1 pokój na 5 minut
                    rateLimitPolicyBuilder.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    rateLimitPolicyBuilder.QueueLimit = 0; // Bez kolejki
                    rateLimitPolicyBuilder.ReplenishmentPeriod = TimeSpan.FromMinutes(5); // Czas odnowienia tokena
                    rateLimitPolicyBuilder.TokensPerPeriod = 1; // Ilość tokenów na okres
                });
            });
        }
    }
}
