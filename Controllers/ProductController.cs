using IdempotentApi.Application.Services;
using IdempotentApi.Data.Repositories;
using IdempotentApi.Filters;
using IdempotentApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdempotentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductRepository productRepository, IIdempotencyService idempotencyService) : ControllerBase
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IIdempotencyService _idempotencyService = idempotencyService;

        // Primary Contructor int the class declaration (above)
        //public ProductController(IProductRepository productRepository)
        //{
        //    _productRepository = productRepository;
        //}

        [HttpGet("GetProductsById")]
        public async Task<IActionResult> GetProductsByIdAsync([FromQuery] int id) 
        {
            Product? product = await _productRepository.GetProductsByIdAsync(id);
            if (product != null)
                return Ok(product);
            else 
                return BadRequest("Product not found!");
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProductsAsync()
        {
            IEnumerable<Product> products = await _productRepository.GetProductsAsync();
            return Ok(products);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProductAsync([FromBody] Product product)
        {
            Product newProduct = await _productRepository.CreateProductAsync(product);
            return Created();
        }

        [HttpPost("CreateProductIdempotentWithServiceInjection")]
        public async Task<IActionResult> CreateProductAsyncIdempotent([FromHeader(Name = "Idempotency-Key")] 
                                                                            string? idempotencyKey, 
                                                                      [FromBody] Product product)
        {
            ActionResult result = _idempotencyService.IdempotencyValidator(idempotencyKey);

            if (result is BadRequestObjectResult || result is OkResult)
                return result;

            Product newProduct = await _productRepository.CreateProductAsync(product);

            return CreatedAtAction(nameof(CreateProductAsyncIdempotent), new { id = newProduct.Id }, newProduct);
        }

        [HttpPost("CreateProductIdempotentWithFilter")]
        [ServiceFilter(typeof(IdempotencyFilter))]
        public async Task<IActionResult> CreateProductAsyncIdempotentWithFilter([FromHeader(Name = "Idempotency-Key")]
                                                                                        string? idempotencyKey, 
                                                                                [FromBody] Product product)
        {
            Product newProduct = await _productRepository.CreateProductAsync(product);

            return CreatedAtAction(nameof(CreateProductAsyncIdempotent), new { id = newProduct.Id }, newProduct);
        }
    }
}
