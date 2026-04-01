using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoucherService.Service;
using VoucherService.Service.DTO.Request;

namespace VoucherService.API.Controllers;

[Route("api/voucher-detail")]
[ApiController]
public class VoucherDetailController : ControllerBase
{
    private readonly IVoucherDetailService _service;

    public VoucherDetailController(IVoucherDetailService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var response = await _service.GetByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("voucher/{voucherId:int}")]
    public async Task<IActionResult> GetByVoucherId([FromRoute] int voucherId)
    {
        var response = await _service.GetByVoucherIdAsync(voucherId);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVoucherDetailRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _service.CreateAsync(request);
        return StatusCode(response.StatusCode, response);
    }
}