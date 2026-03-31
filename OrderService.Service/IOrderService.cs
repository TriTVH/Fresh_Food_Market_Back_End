using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;

namespace OrderService.Service
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderDTO>> CreateOrderAsync(CreateOrderModel request, string accountUsername);
        Task<ApiResponse<OrderDTO>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<OrderDTO>> GetOrderByNumberAsync(string orderNumber);
        Task<ApiResponse<OrderDTO>> ConfirmPaymentAsync(int orderId);
        Task<ApiResponse<OrderDTO>> ProcessVnpayIpnAsync(string orderNumber, bool paymentSuccess, string vnpTransactionNo);
    }
}
