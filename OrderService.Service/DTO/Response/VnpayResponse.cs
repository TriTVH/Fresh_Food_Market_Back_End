namespace OrderService.Service.DTO.Response
{
    public class VnpayPaymentUrlResponse
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = default!;
        public decimal Amount { get; set; }
        public string PaymentUrl { get; set; } = default!;
    }

    public class VnpayIpnResponse
    {
        public string RspCode { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
