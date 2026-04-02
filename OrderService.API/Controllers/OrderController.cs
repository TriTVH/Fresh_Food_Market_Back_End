using JwtConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OrderService.Service;
using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

namespace OrderService.API.Controllers
{
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
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderModel request)
        {
            var response = await _orderService.CreateOrderAsync(request, GetUsername(), GetIpAddress(HttpContext));
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateOrderModel request)
        {
            var response = await _orderService.UpdateOrderAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        //[HttpPatch("{orderId}/confirm-payment")]
        //public async Task<IActionResult> ConfirmPayment(int orderId)
        //{
        //    var response = await _orderService.ConfirmPaymentAsync(orderId);
        //    return StatusCode(response.StatusCode, response);
        //}
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByUsername()
        {
            var response = await _orderService.GetOrdersByUsernameAsync(GetUsername());
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var response = await _orderService.GetOrders();
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] CancelOrderRequest request)
        {
            var response = await _orderService.CancelOrderAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        private string? GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("username")?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        }
        public static string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "127.0.0.1";
        }


    }

}