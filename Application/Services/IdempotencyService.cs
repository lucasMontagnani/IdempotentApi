using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IdempotentApi.Application.Services
{
    public class IdempotencyService : IIdempotencyService
    {
        // Replace with a distributed cache in production (e.g., Redis)
        private readonly IMemoryCache _memoryCache;

        public IdempotencyService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool ValidateIdempotencyKey(Guid idempotencyKey)
        {
            // Verify if the IdempotencyKey is already in the cache
            if (!_memoryCache.TryGetValue(idempotencyKey, out var result)) 
            {
                // Guardar a chave no cache
                //var cacheEntryOptions = new MemoryCacheEntryOptions()
                //        .SetAbsoluteExpiration(TimeSpan.FromHours(10))
                //        .SetPriority(CacheItemPriority.Normal);
                //_memoryCache.Set(idempotencyKey, _, cacheEntryOptions);

                return true;
            }
            return false;
        }

        public ActionResult IdempotencyValidator(string? idempotencyString)
        {
            if (string.IsNullOrEmpty(idempotencyString))
                return new BadRequestObjectResult("Idempotency-Key header is required.");

            if (!Guid.TryParse(idempotencyString, out Guid idempotencyKey))
                return new BadRequestObjectResult("Invalid Idempotency-Key, it needs to be a GUID.");

            // Check if the key already exists in the cache
            if (_memoryCache.TryGetValue(idempotencyKey, out _))
            {
                return new OkResult(); // Return 200 OK if the key already exists
            }

            // Store the key in the cache to prevent reprocessing
            _memoryCache.Set(idempotencyKey, true, TimeSpan.FromHours(10));

            return new CreatedResult("", null); // Return 201 Created when processing for the first time
        }


        public Task<IActionResult?> GetResponseAsync(string idempotencyKey)
        {
            if (_memoryCache.TryGetValue(idempotencyKey, out var cachedResponse))
            {
                return Task.FromResult(cachedResponse as IActionResult);
            }

            return Task.FromResult<IActionResult?>(null);
        }

        public Task SaveResponseAsync(string idempotencyKey, object response)
        {
            // Cache the response for a specific duration (e.g., 10 hours)
            _memoryCache.Set(idempotencyKey, new OkObjectResult(response), TimeSpan.FromHours(10));
            return Task.CompletedTask;
        }
    }
}
