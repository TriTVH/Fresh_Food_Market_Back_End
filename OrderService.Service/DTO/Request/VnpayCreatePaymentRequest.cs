namespace OrderService.Service.DTO.Request
{
    public class VnpayCreatePaymentRequest
    {
        public string OrderNumber { get; set; } = default!;
    }

    public class VnpaySimulateIpnRequest
    {
        /// <summary>
        /// OrderNumber (vnp_TxnRef) của đơn hàng cần simulate.
        /// </summary>
        public string OrderNumber { get; set; } = default!;

        /// <summary>
        /// true = thanh toán thành công, false = thất bại.
        /// </summary>
        public bool Success { get; set; } = true;
    }
}
