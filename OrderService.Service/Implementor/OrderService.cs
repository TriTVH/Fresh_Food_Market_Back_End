using OrderService.Repository;
using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using OrderService.Service.HttpClients;
using OrderService.Model;
using OrderService.Model.Entities;

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
                //var saga = new CreateOrder(_orderRepo, _productHttpClient);
                //var result = await saga.ExecuteAsync(request);
                return ApiResponse<OrderDTO>.Ok(null, "Order created successfully", 201);
            }
            catch(InvalidOperationException ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 400);
            }
            catch(Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }

        //public async Task<ApiResponse<List<OrderDTO>>> GetAllOrdersAsync()
        //{
        //    try
        //    {
        //        var orders = await _orderRepo.GetAllAsync();
        //        var result = orders.Select(MapToDTO).ToList();
        //        return ApiResponse<List<OrderDTO>>.Ok(result, "Orders retrieved successfully", 200);
        //    }
        //    catch(Exception ex)
        //    {
        //        return ApiResponse<List<OrderDTO>>.Error(null, ex.Message, 500);
        //    }
        //}

        //public async Task<ApiResponse<OrderDTO>> GetOrderByIdAsync(int orderId)
        //{
        //    try
        //    {
        //        var order = await _orderRepo.GetByIdAsync(orderId);
        //        if(order == null)
        //            return ApiResponse<OrderDTO>.Error(null, $"Order {orderId} not found", 404);

        //        return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "Order retrieved successfully", 200);
        //    }
        //    catch(Exception ex)
        //    {
        //        return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
        //    }
        //}

        //public async Task<ApiResponse<OrderDTO>> CancelOrderAsync(int orderId, string? cancelReason)
        //{
        //    try
        //    {
        //        var order = await _orderRepo.GetByIdAsync(orderId);
        //        if(order == null)
        //            return ApiResponse<OrderDTO>.Error(null, $"Order {orderId} not found", 404);

        //        if(order.Status == "cancelled")
        //            throw new InvalidOperationException("Order is already cancelled");

        //        order.Status = "cancelled";
        //        order.CancelledDate = DateTime.UtcNow;
        //        order.CancelReason = cancelReason;
        //        order.UpdatedDate = DateTime.UtcNow;

        //        await _orderRepo.UpdateAsync(order);
        //        return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "Order cancelled successfully", 200);
        //    }
        //    catch(InvalidOperationException ex)
        //    {
        //        return ApiResponse<OrderDTO>.Error(null, ex.Message, 400);
        //    }
        //    catch(Exception ex)
        //    {
        //        return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
        //    }
        //}

        private static OrderDTO MapToDTO(Order order) => new()
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Items = order.OrderDetails.Select(d => new OrderDetailDTO
            {
                OrderDetailId = d.OrderDetailId,
                ProductId = d.ProductId,
                ProductName = d.ProductName,
                Quantity = d.Quantity,
                Subtotal = d.Subtotal,
            }).ToList(),
        };
    }
}