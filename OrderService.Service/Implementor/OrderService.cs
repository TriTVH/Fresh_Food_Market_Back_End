using OrderService.Repository;
using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using OrderService.Service.HttpClients;
using OrderService.Service.Saga;

namespace OrderService.Service.Implementor
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IProductHttpClient _productHttpClient;

        public OrderService(IOrderRepo orderRepo, IProductHttpClient productHttpClient)
        {
            _orderRepo = orderRepo;
            _productHttpClient = productHttpClient;
        }

        public async Task<ApiResponse<OrderDTO>> CreateOrderAsync(CreateOrderModel request)
        {
            try
            {
                var saga = new CreateOrderSaga(_orderRepo, _productHttpClient);
                var result = await saga.ExecuteAsync(request);
                return ApiResponse<OrderDTO>.Ok(result, "Order created successfully", 201);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }
    }
}
