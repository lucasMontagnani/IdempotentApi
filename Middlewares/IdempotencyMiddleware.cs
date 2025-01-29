using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IdempotentApi.Middlewares
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;

        public IdempotencyMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            _next = next;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Idempotency-Key header is required.");
                return;
            }

            //if (string.IsNullOrEmpty(idempotencyKey))
            //{
            //    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            //    await context.Response.WriteAsync("Idempotency-Key header is required.");
            //    return;
            //}

            if (!Guid.TryParse(idempotencyKey, out Guid idempotencyGuid))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid Idempotency-Key, it needs to be a GUID.");
                return;
            }

            if (_memoryCache.TryGetValue(idempotencyGuid, out _))
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return;
            }

            _memoryCache.Set(idempotencyGuid, true, TimeSpan.FromHours(10));

            await _next(context);
        }
    }

}
