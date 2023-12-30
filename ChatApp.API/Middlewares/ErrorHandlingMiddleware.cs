using System.Security.Authentication;

namespace ChatApp.API.Middlewares
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch(InvalidCredentialException credentialException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync($"{credentialException.Message}");
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized access");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
