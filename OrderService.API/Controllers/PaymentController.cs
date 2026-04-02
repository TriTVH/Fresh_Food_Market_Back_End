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

        public PaymentController(IOrderService orderService, IWebHostEnvironment env)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPAY — client redirect đến URL này để thanh toán.
        /// </summary>
        //[Authorize]
        //[HttpPost("create")]
        //public async Task<IActionResult> CreatePaymentUrl([FromBody] VnpayCreatePaymentRequest request)
        //{

        //    var ipAddress = GetClientIp();
        //    var orderInfo = $"Thanh toan don hang";

        //    var paymentUrl = _vnpayService.CreatePaymentUrl(
        //        request.TotalAmount,
        //        orderInfo,
        //        ipAddress);

        //    return Ok(new VnpayPaymentUrlResponse
        //    {
        //        PaymentUrl = paymentUrl
        //    });
        //}

        ///// <summary>
        ///// IPN URL — VNPAY gọi server-to-server sau khi thanh toán xong.
        ///// Cấu hình URL này trên VNPAY Merchant Admin.
        ///// </summary>
        //[HttpGet("ipn")]
        //public async Task<IActionResult> Ipn()
        //{
        //    var query = ToDict(Request.Query);

        //    if (!_vnpayService.ValidateSignature(query))
        //        return BadRequest(new VnpayIpnResponse { RspCode = "97", Message = "Invalid signature" });

        //    query.TryGetValue("vnp_TxnRef", out var txnRef);
        //    query.TryGetValue("vnp_ResponseCode", out var responseCode);
        //    query.TryGetValue("vnp_TransactionNo", out var transactionNo);
        //    query.TryGetValue("vnp_TransactionStatus", out var transactionStatus);

        //    var isSuccess = responseCode == "00" && transactionStatus == "00";

        //    return Ok();
        //}

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmPaymentUrl([FromQuery] string vnp_TxnRef, [FromQuery] string vnp_ResponseCode, [FromQuery] string vnp_Amount)
        {

            var result = await _orderService.ProcessVnPay(vnp_TxnRef, vnp_ResponseCode);

            return StatusCode(result.StatusCode, result);
        }

        ///// <summary>
        ///// Return URL — VNPAY redirect browser khách hàng sau khi thanh toán.
        ///// </summary>
        //[HttpGet("return")]
        //public IActionResult Return()
        //{
        //    var query = ToDict(Request.Query);

        //    if (!_vnpayService.ValidateSignature(query))
        //        return BadRequest(new { message = "Invalid signature" });

        //    query.TryGetValue("vnp_ResponseCode", out var responseCode);
        //    query.TryGetValue("vnp_TxnRef", out var txnRef);
        //    query.TryGetValue("vnp_Amount", out var amount);

        //    if (responseCode == "00")
        //        return Ok(new { message = "Thanh toán thành công", orderNumber = txnRef, amount });

        //    return BadRequest(new { message = "Thanh toán thất bại", responseCode, orderNumber = txnRef });
        //}

    

        ///// <summary>
        ///// [CHỈ DÙNG KHI DEVELOPMENT] Simulate IPN không cần URL public.
        ///// Gọi thẳng vào ProcessVnpayIpnAsync để test luồng thanh toán cục bộ.
        ///// </summary>
        //[HttpPost("dev/simulate")]
        //public async Task<IActionResult> SimulateIpn([FromBody] VnpaySimulateIpnRequest request)
        //{
        //    if (!_env.IsDevelopment())
        //        return NotFound();

        //    var result = await _orderService.ProcessVnpayIpnAsync(
        //        request.OrderNumber,
        //        request.Success,
        //        request.Success ? "SIMULATED_SUCCESS" : "SIMULATED_FAILED");

        //    return StatusCode(result.StatusCode, result);
        //}
    }
}
