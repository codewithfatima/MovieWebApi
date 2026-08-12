namespace MovieWebApi.Middleware
{
    public class MiddlewareHandler
    {

      private readonly RequestDelegate _next;
        private readonly ILogger<MiddlewareHandler> _logger;

        public MiddlewareHandler(RequestDelegate next , ILogger<MiddlewareHandler> logger )
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
               _logger.LogError(ex, "Unhandled exception occured");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred.\"}");
            }
        }
    }
}
