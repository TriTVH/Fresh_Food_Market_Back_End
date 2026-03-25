using Microsoft.AspNetCore.Mvc;
using OrderService.Service;
using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;

namespace OrderService.API.Controllers
{
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderModel request)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                return BadRequest(ApiResponse<object>.Error(null, firstError, 400));
            }

            var response = await _orderService.CreateOrderAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
