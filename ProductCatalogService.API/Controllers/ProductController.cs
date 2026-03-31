using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.Service;
using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Request;

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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var response = await _productService.GetProductByIdAsync(id);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProducts()
        {
            var response = await _productService.GetActiveProducts();

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

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel request)
        {

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                return BadRequest(ApiResponse<object>.Error(null, firstError, 400));
            }
            var response = await _productService.UpdateProductAsync(request);
            if (response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }

        [HttpPut("quantity")]
        public async Task<IActionResult> UpdateProductQty([FromBody] UpdateProductQtyRequest request)
        {
            
            var response = await _productService.UpdateProductQtyAsync(request);
     
                return StatusCode(response.StatusCode, response);
            
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    var response = await _productService.DeleteProduct(id);
        //    return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        //}

        //[HttpGet("search")]
        //public async Task<IActionResult> GetListProducts([FromQuery] string? search)
        //{
        //    var response = await _productService.GetAllProducts(search);

        //    if (response.Success)
        //    {
        //        return Ok(response);
        //    }
        //    return StatusCode(response.StatusCode, response);
        //}
    }
}
