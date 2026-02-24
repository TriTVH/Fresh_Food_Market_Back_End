using Microsoft.AspNetCore.Mvc;
using ProductCatalogService.Service;
using ProductCatalogService.Service.DTO.Request;

namespace ProductCatalogService.API.Controllers
{
    [Route("api/supplier")]
    public class SupplierController : ControllerBase
    {
        ISupplierService _supplierService;
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierModel supplierModel)
        {
            var response = await _supplierService.CreateSupplierAsync(supplierModel);
           
            if (response.Success)
                return Ok(response);
            else
                return StatusCode(response.StatusCode, response);
        }
    }
}
