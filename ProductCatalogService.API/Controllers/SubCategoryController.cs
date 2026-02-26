using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.Service;

namespace ProductCatalogService.API.Controllers
{
    [Route("api/sub-category")]
    public class SubCategoryController : Controller
    {
        public ISubCategoryService _subCategoryService;
        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }
        [HttpGet]
        public IActionResult GetSubCategories()
        {
            var response = _subCategoryService.GetSubCategories().Result;
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
    }
}
