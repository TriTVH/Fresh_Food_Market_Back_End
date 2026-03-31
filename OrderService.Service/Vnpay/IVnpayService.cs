namespace OrderService.Service.Vnpay
{
    public interface IVnpayService
    {
        /// <summary>
        /// Tạo URL thanh toán VNPAY để redirect khách hàng.
        /// </summary>
        string CreatePaymentUrl(string orderNumber, decimal amount, string orderInfo, string ipAddress);

        /// <summary>
        /// Xác thực chữ ký từ callback IPN/Return của VNPAY.
        /// </summary>
        bool ValidateSignature(IDictionary<string, string> query);
    }
}
