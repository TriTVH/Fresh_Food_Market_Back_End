using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;

namespace OrderService.Service
{
    public interface IOrderService
    {

        Task<ApiResponse<string>> CreateOrderAsync(CreateOrderModel request, string accUsername, string ipAddress);
        Task<ApiResponse<OrderDTO>> UpdateOrderAsync(UpdateOrderModel request);
        Task<ApiResponse<List<OrderDTO>>> GetOrdersByUsernameAsync(string accUsername);
        Task<ApiResponse<OrderDTO>> GetOrderByIdAsync(int orderId);
        Task<ApiResponse<List<OrderDTO>>> GetOrders();

        Task<ApiResponse<bool>> CancelOrderAsync(CancelOrderRequest request);
        Task<ApiResponse<bool>> ProcessVnPay(string vnp_TxnRef, string vnp_ResponseCode);

    }
}
