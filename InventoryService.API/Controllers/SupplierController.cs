﻿using InventoryService.Service;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers
{
    [Route("api/supplier")]
    public class SupplierController : ControllerBase
    {
        ISupplierService _service;
        public SupplierController(ISupplierService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _service.GetSuppliers();

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
        public async Task<IActionResult> Create([FromBody] InventoryService.Service.DTO.Request.CreateSupplierModel model)
        {
            var response = await _service.CreateSupplierAsync(model);
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
