using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IdempotentApi.Filters
{
    public class IdempotencyFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache _memoryCache;

        public IdempotencyFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey))
            {
                context.Result = new BadRequestObjectResult("Idempotency-Key header is required.");
                return;
            }

            if (string.IsNullOrEmpty(idempotencyKey))
            {
                context.Result = new BadRequestObjectResult("Idempotency-Key header is required.");
                return;
            }

            if (!Guid.TryParse(idempotencyKey, out Guid idempotencyGuid))
            {
                context.Result = new BadRequestObjectResult("Invalid Idempotency-Key, it needs to be a GUID.");
                return;
            }

            if (_memoryCache.TryGetValue(idempotencyGuid, out _))
            {
                context.Result = new OkResult();
                return;
            }

            _memoryCache.Set(idempotencyGuid, true, TimeSpan.FromHours(10));

            await next();
        }
    }

}
