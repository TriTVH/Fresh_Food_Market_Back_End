using InventoryService.Service;
using InventoryService.Service.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryService.API.Controllers
{
    [Route("api/batch")]
    public class BatchController : ControllerBase
    {
        IBatchService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BatchController(IBatchService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var response = await _service.GetAllBatchesAsync();

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(response.StatusCode, response);
            }
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await _service.GetBatchByIdAsync(id);

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
        public async Task<ActionResult> Create([FromBody] CreateBatchModel request)
        {
            var response = await _service.AddBatchAsync(request);

                return StatusCode(response.StatusCode, response);
            
        }
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateBatchModel request)
        {
            var response = await _service.UpdateBatchAsync(request, GetUsername(), GetRole());

            return StatusCode(response.StatusCode, response);

        }

        private string? GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value;
        }

        private string? GetRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;
        }
    }
}
