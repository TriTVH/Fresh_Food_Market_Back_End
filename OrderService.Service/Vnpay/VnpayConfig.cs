namespace OrderService.Service.Vnpay
{
    public class VnpayConfig
    {
        public string TmnCode { get; set; } = default!;
        public string HashSecret { get; set; } = default!;
        public string PaymentUrl { get; set; } = default!;
        public string ReturnUrl { get; set; } = default!;
    }
}
