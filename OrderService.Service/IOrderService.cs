using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;

namespace OrderService.Service
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderDTO>> CreateOrderAsync(CreateOrderModel request);
    }
}
