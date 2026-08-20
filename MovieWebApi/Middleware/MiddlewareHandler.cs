namespace MovieWebApi.Middleware
{
    public class MiddlewareHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareHandler> _logger;

        public MiddlewareHandler(
            RequestDelegate next,
            ILogger<MiddlewareHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "UNHANDLED ERROR: {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path
                );

                // IMPORTANT: log the inner exception too
                if (ex.InnerException != null)
                {
                    _logger.LogError(
                        "INNER EXCEPTION: {Message}",
                        ex.InnerException.Message
                    );
                }

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred."
                });
            }
        }
    }
}