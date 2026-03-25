using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoucherService.Service;
using VoucherService.Service.DTO.Request;

namespace VoucherService.API.Controllers;

[Route("api/voucher")]
[ApiController]
public class VoucherController : ControllerBase
{
    private readonly IVoucherService _voucherService;

    public VoucherController(IVoucherService voucherService)
    {
        _voucherService = voucherService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var response = await _voucherService.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var response = await _voucherService.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateVoucherRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _voucherService.CreateAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdateVoucherRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _voucherService.UpdateAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var response = await _voucherService.DeleteAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}
