namespace ChatApp.API.Middlewares
{
    public class UserAgentMiddleware
    {
        private readonly RequestDelegate _next;

        public UserAgentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrWhiteSpace(userAgent))
            {
                // Handle the case when the User-Agent header is missing or empty.
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("User-Agent header is required.");
                return;
            }

            // If the User-Agent header is present, continue processing the request.
            await _next(context);
        }
    }
}
