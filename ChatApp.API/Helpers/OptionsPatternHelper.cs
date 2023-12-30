using ChatApp.Common.ConfigurationHelpers;

namespace ChatApp.API.Helpers
{
    public static class OptionsPatternHelper
    {
        public static void OptionsPattern(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<JwtBearerConfig>(builder.Configuration.GetSection("Authentication:Schemes:Bearer"));
        }
    }
}
