using InventoryService.Service;
using InventoryService.Service.DTO.Request;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers
{
    [Route("api/batch")]
    public class BatchController : ControllerBase
    {
        IBatchService _service;
       
        public BatchController(IBatchService service)
        {
            _service = service;
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

    }
}
