using JwtConfiguration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderService.Service;
using OrderService.Service.DTO.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderModel request)
        {
            var response = await _orderService.CreateOrderAsync(request, GetUsername());
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("{orderId}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            var response = await _orderService.ConfirmPaymentAsync(orderId);
            return StatusCode(response.StatusCode, response);
        }

        private string? GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value;
        }

    }
 
}