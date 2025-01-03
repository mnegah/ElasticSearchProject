using ElasticSearchSampleProject.Application.Features;
using ElasticSearchSampleProject.Application.Services;
using ElasticSearchSampleProject.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearchSampleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string term)
        {
            var results = await _productService.IndexProductsAsync(term);
            return Ok(results);
        }
    }
}
