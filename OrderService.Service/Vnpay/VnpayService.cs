using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace OrderService.Service.Vnpay
{
    public class VnpayService : IVnpayService
    {
        private readonly VnpayConfig _config;

        public VnpayService(IOptions<VnpayConfig> options)
        {
            _config = options.Value;
        }

        public string CreatePaymentUrl(string orderNumber, decimal amount, string orderInfo, string ipAddress)
        {
            // VNPAY yêu cầu thời gian theo múi giờ Việt Nam (UTC+7)
            var now = DateTime.UtcNow.AddHours(7);

            var vnpParams = new SortedDictionary<string, string>
            {
                ["vnp_Version"]    = "2.1.0",
                ["vnp_Command"]    = "pay",
                ["vnp_TmnCode"]    = _config.TmnCode,
                ["vnp_Amount"]     = ((long)(amount * 100)).ToString(),
                ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"]   = "VND",
                ["vnp_IpAddr"]     = ipAddress,
                ["vnp_Locale"]     = "vn",
                ["vnp_OrderInfo"]  = orderInfo,
                ["vnp_OrderType"]  = "other",
                ["vnp_ReturnUrl"]  = _config.ReturnUrl,
                ["vnp_TxnRef"]     = orderNumber,
                ["vnp_ExpireDate"] = now.AddMinutes(15).ToString("yyyyMMddHHmmss"),
            };

            // Build raw data để hash (không encode)
            var rawData = string.Join("&", vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
            var secureHash = HmacSha512(_config.HashSecret, rawData);

            // Build final URL (encode value cho URL)
            var queryString = string.Join("&", vnpParams.Select(kv =>
                $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

            return $"{_config.PaymentUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateSignature(IDictionary<string, string> query)
        {
            if (!query.TryGetValue("vnp_SecureHash", out var receivedHash) || string.IsNullOrEmpty(receivedHash))
                return false;

            // Lấy tất cả param vnp_ trừ vnp_SecureHash, sắp xếp theo key
            var rawData = string.Join("&", query
                .Where(kv => kv.Key.StartsWith("vnp_") && kv.Key != "vnp_SecureHash")
                .OrderBy(kv => kv.Key)
                .Select(kv => $"{kv.Key}={kv.Value}"));

            var expectedHash = HmacSha512(_config.HashSecret, rawData);
            return receivedHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        private static string HmacSha512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            return Convert.ToHexString(hmac.ComputeHash(dataBytes)).ToLower();
        }
    }
}
