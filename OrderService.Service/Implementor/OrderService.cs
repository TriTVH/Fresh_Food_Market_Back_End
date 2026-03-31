using OrderService.Repository;
using OrderService.Service.DTO;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using OrderService.Service.HttpClients;
using OrderService.Model.Entities;
using OrderService.Service.Saga.Orchestator;
using OrderService.Service.Saga.Orchestator.Steps;
using Microsoft.EntityFrameworkCore;
using OrderService.Model.DBContext;

namespace OrderService.Service.Implementor
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IProductHttpClient _productHttpClient;
        private readonly OrderMgmtFfmContext _context;

        public OrderService(IOrderRepo orderRepo, ITransactionRepo transactionRepo, IProductHttpClient productHttpClient, OrderMgmtFfmContext context)
        {
            _orderRepo = orderRepo;
            _transactionRepo = transactionRepo;
            _productHttpClient = productHttpClient;
            _context = context;
        }

        public async Task<ApiResponse<OrderDTO>> CreateOrderAsync(CreateOrderModel request, string accountUsername)
        {
            try
            {
                var saga = new SagaOrchestrator()
                    .AddStep(new CreateOrderStep(request, _orderRepo, _productHttpClient, accountUsername))
                    .AddStep(new CreateTransactionStep(_transactionRepo));

                var sagaContext = await saga.ExecuteAsync();

                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Transactions)
                    .FirstOrDefaultAsync(o => o.OrderId == sagaContext.OrderId);

                return ApiResponse<OrderDTO>.Ok(MapToDTO(order!), "Order created successfully", 201);
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

        public async Task<ApiResponse<OrderDTO>> ProcessVnpayIpnAsync(string orderNumber, bool paymentSuccess, string vnpTransactionNo)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Transactions)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

                if (order == null)
                    return ApiResponse<OrderDTO>.Error(null, $"Order '{orderNumber}' not found", 404);

                var transaction = order.Transactions
                    .FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");

                if (transaction == null)
                    return ApiResponse<OrderDTO>.Error(null, "No pending payment transaction found", 400);

                if (paymentSuccess)
                {
                    transaction.Status = "SUCCESS";
                    order.Status = "CONFIRMED";
                    order.ConfirmedDate = DateTime.UtcNow;
                }
                else
                {
                    transaction.Status = "FAILED";
                    order.Status = "CANCELLED";
                    order.CancelReason = $"Payment failed — VNPAY txn: {vnpTransactionNo}";
                    order.CancelledDate = DateTime.UtcNow;
                }

                order.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return ApiResponse<OrderDTO>.Ok(MapToDTO(order), paymentSuccess ? "Payment successful" : "Payment failed", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }

        public async Task<ApiResponse<OrderDTO>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Transactions)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    return ApiResponse<OrderDTO>.Error(null, $"Order {orderId} not found", 404);

                return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "OK", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }

        public async Task<ApiResponse<OrderDTO>> GetOrderByNumberAsync(string orderNumber)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Transactions)
                    .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

                if (order == null)
                    return ApiResponse<OrderDTO>.Error(null, $"Order '{orderNumber}' not found", 404);

                return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "OK", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }

        public async Task<ApiResponse<OrderDTO>> ConfirmPaymentAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.Transactions)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    return ApiResponse<OrderDTO>.Error(null, $"Order {orderId} not found", 404);

                var transaction = order.Transactions
                    .FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");

                if (transaction == null)
                    return ApiResponse<OrderDTO>.Error(null, "No pending payment transaction found for this order", 400);

                if (order.Status != "PENDING")
                    return ApiResponse<OrderDTO>.Error(null, $"Order is already in status '{order.Status}'", 400);

                transaction.Status = "SUCCESS";
                order.Status = "CONFIRMED";
                order.ConfirmedDate = DateTime.UtcNow;
                order.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "Payment confirmed successfully", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }

        private static OrderDTO MapToDTO(Order order)
        {
            var paymentTx = order.Transactions
                .FirstOrDefault(t => t.Type == "PAYMENT");

            return new OrderDTO
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                TransactionId = paymentTx?.Id,
                TransactionStatus = paymentTx?.Status,
                Items = order.OrderDetails.Select(d => new OrderDetailDTO
                {
                    OrderDetailId = d.OrderDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    Subtotal = d.Subtotal,
                }).ToList()
            };
        }
    }
}

