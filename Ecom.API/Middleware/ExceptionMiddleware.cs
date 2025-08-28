using Ecom.API.Helper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Text.Json;

namespace Ecom.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(30);

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment, IMemoryCache memoryCache)
        {
            _next = next;
            _environment = environment;
            _memoryCache = memoryCache;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecurity(context);
                if(IsRequestAllowed(context) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";
                    var Response = new ApiException((int)HttpStatusCode.InternalServerError, "Too many requests , try again later");
                    await context.Response.WriteAsJsonAsync(Response);

                }
                await _next(context);
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var Response = _environment.IsDevelopment() ?
                    new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace)
                    : new ApiException((int)HttpStatusCode.InternalServerError, ex.Message);
                var json = JsonSerializer.Serialize(Response);
                await context.Response.WriteAsync(json);
            }
        }
        private bool IsRequestAllowed(HttpContext context)
        {
            var Ip = context.Connection.RemoteIpAddress.ToString();
            var CachKey = $"Rate:{Ip}";
            var DateNow = DateTime.Now;
            var (timeStamp, count) = _memoryCache.GetOrCreate(CachKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _rateLimitWindow;
                return (timeStamp: DateNow, count: 0);
            });
            if(DateNow - timeStamp < _rateLimitWindow)
            {
                if (count >= 8)
                    return false;
                _memoryCache.Set(CachKey, (timeStamp, count += 1), _rateLimitWindow);
            }
            else
            {
                _memoryCache.Set(CachKey, (timeStamp, count += 1), _rateLimitWindow);

            }
            return true; 
        }
        private void ApplySecurity(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Ptotection"] = "1;mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }
    }
}
