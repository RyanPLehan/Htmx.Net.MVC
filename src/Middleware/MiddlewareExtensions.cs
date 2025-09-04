namespace ContosoUniversity.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseHtmxProcessor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HtmxProcessor>();
        }
    }
}
