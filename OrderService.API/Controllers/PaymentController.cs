using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Service;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using OrderService.Service.Vnpay;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/payment/vnpay")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVnpayService _vnpayService;
        private readonly IWebHostEnvironment _env;

        public PaymentController(IOrderService orderService, IVnpayService vnpayService, IWebHostEnvironment env)
        {
            _orderService = orderService;
            _vnpayService = vnpayService;
            _env = env;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPAY — client redirect đến URL này để thanh toán.
        /// </summary>
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] VnpayCreatePaymentRequest request)
        {
            var orderResult = await _orderService.GetOrderByNumberAsync(request.OrderNumber);
            if (!orderResult.Success)
                return StatusCode(orderResult.StatusCode, orderResult);

            var order = orderResult.Data!;

            if (order.Status != "PENDING")
                return BadRequest(new { message = $"Order is not eligible for payment (status: {order.Status})" });

            var ipAddress = GetClientIp();
            var orderInfo = $"Thanh toan don hang {order.OrderNumber}";

            var paymentUrl = _vnpayService.CreatePaymentUrl(
                order.OrderNumber,
                order.TotalAmount,
                orderInfo,
                ipAddress);

            return Ok(new VnpayPaymentUrlResponse
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Amount = order.TotalAmount,
                PaymentUrl = paymentUrl
            });
        }

        /// <summary>
        /// IPN URL — VNPAY gọi server-to-server sau khi thanh toán xong.
        /// Cấu hình URL này trên VNPAY Merchant Admin.
        /// </summary>
        [HttpGet("ipn")]
        public async Task<IActionResult> Ipn()
        {
            var query = ToDict(Request.Query);

            if (!_vnpayService.ValidateSignature(query))
                return Ok(new VnpayIpnResponse { RspCode = "97", Message = "Invalid signature" });

            query.TryGetValue("vnp_TxnRef", out var txnRef);
            query.TryGetValue("vnp_ResponseCode", out var responseCode);
            query.TryGetValue("vnp_TransactionNo", out var transactionNo);
            query.TryGetValue("vnp_TransactionStatus", out var transactionStatus);

            // Thành công khi cả responseCode và transactionStatus đều "00"
            var isSuccess = responseCode == "00" && transactionStatus == "00";

            var result = await _orderService.ProcessVnpayIpnAsync(txnRef ?? "", isSuccess, transactionNo ?? "");

            if (result.StatusCode == 404)
                return Ok(new VnpayIpnResponse { RspCode = "01", Message = "Order not found" });

            if (result.StatusCode == 400)
                return Ok(new VnpayIpnResponse { RspCode = "02", Message = result.Message });

            return Ok(new VnpayIpnResponse { RspCode = "00", Message = "Confirm Success" });
        }

        /// <summary>
        /// Return URL — VNPAY redirect browser khách hàng sau khi thanh toán.
        /// </summary>
        [HttpGet("return")]
        public IActionResult Return()
        {
            var query = ToDict(Request.Query);

            if (!_vnpayService.ValidateSignature(query))
                return BadRequest(new { message = "Invalid signature" });

            query.TryGetValue("vnp_ResponseCode", out var responseCode);
            query.TryGetValue("vnp_TxnRef", out var txnRef);
            query.TryGetValue("vnp_Amount", out var amount);

            if (responseCode == "00")
                return Ok(new { message = "Thanh toán thành công", orderNumber = txnRef, amount });

            return BadRequest(new { message = "Thanh toán thất bại", responseCode, orderNumber = txnRef });
        }

        private string GetClientIp()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            return string.IsNullOrEmpty(ip) ? "127.0.0.1" : ip;
        }

        private static Dictionary<string, string> ToDict(IQueryCollection query)
            => query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

        /// <summary>
        /// [CHỈ DÙNG KHI DEVELOPMENT] Simulate IPN không cần URL public.
        /// Gọi thẳng vào ProcessVnpayIpnAsync để test luồng thanh toán cục bộ.
        /// </summary>
        [HttpPost("dev/simulate")]
        public async Task<IActionResult> SimulateIpn([FromBody] VnpaySimulateIpnRequest request)
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var result = await _orderService.ProcessVnpayIpnAsync(
                request.OrderNumber,
                request.Success,
                request.Success ? "SIMULATED_SUCCESS" : "SIMULATED_FAILED");

            return StatusCode(result.StatusCode, result);
        }
    }
}
