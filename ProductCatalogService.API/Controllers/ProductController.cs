using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.Service;
using ProductCatalogService.Service.DTO;

namespace ProductCatalogService.API.Controllers
{
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        public IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetAllProducts();

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Service.DTO.Request.CreateProductModel request)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                return BadRequest(ApiResponse<object>.Error(null, firstError, 400));
            }
            var response = await _productService.CreateProduct(request);
            if (response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}
