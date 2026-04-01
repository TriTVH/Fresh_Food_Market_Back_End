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

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var response = await _orderService.GetAllOrdersAsync();
        //    return StatusCode(response.StatusCode, response);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var response = await _orderService.GetOrderByIdAsync(id);
        //    return StatusCode(response.StatusCode, response);
        //}

        //[HttpPut("{id}/cancel")]
        //public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderRequest request)
        //{
        //    var response = await _orderService.CancelOrderAsync(id, request.CancelReason);
        //    return StatusCode(response.StatusCode, response);
        //}

        private string? GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value;
        }

    }
 
}