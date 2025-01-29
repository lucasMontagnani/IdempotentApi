using Microsoft.AspNetCore.Mvc;

namespace IdempotentApi.Application.Services
{
    public interface IIdempotencyService
    {
        ActionResult IdempotencyValidator(string? idempotencyString);
    }
}
