namespace OrderService.Service.DTO.Response
{
    public class VnpayPaymentUrlResponse
    {
        public string PaymentUrl { get; set; } = default!;
    }

    public class VnpayIpnResponse
    {
        public string RspCode { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
