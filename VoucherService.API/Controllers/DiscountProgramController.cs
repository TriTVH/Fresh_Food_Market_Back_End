using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoucherService.Service;
using VoucherService.Service.DTO.Request;

namespace VoucherService.API.Controllers;

[Route("api/discount-program")]
[ApiController]
public class DiscountProgramController : ControllerBase
{
    private readonly IDiscountProgramService _service;

    public DiscountProgramController(IDiscountProgramService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var response = await _service.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateDiscountProgramRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await _service.CreateAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdateDiscountProgramRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var response = await _service.UpdateAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await _service.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}