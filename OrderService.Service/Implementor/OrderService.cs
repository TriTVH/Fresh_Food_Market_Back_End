using Microsoft.Extensions.Configuration;
using OrderService.Model;
using OrderService.Model.Entities;
using OrderService.Repository;
using OrderService.Service.DTO;
using OrderService.Service.DTO.enums;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using OrderService.Service.HttpClients;
using OrderService.Service.Saga.Orchestator;
using OrderService.Service.Saga.Orchestator.Context;
using OrderService.Service.Saga.Orchestator.Steps;
using OrderService.Service.Vnpay;
using OrderService.Service.Vnpay.Library;
using System.Net;
using System.Runtime.InteropServices;


namespace OrderService.Service.Implementor
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ITransactionRepo _transactionRepo;
        private readonly IProductHttpClient _productHttpClient;
        private readonly IVoucherHttpClient _voucherHttpClient;
        private readonly IConfiguration _configuration;

        public OrderService(IOrderRepo orderRepo, IProductHttpClient productHttpClient, IVoucherHttpClient voucherHttpClient, ITransactionRepo transactionRepo, IConfiguration configuration)

        {
            _orderRepo = orderRepo;
            _transactionRepo = transactionRepo;
            _productHttpClient = productHttpClient;
            _voucherHttpClient = voucherHttpClient;
            _configuration = configuration;
            //_vnpayService = vnpayService;
        }

        public async Task<ApiResponse<string>> CreateOrderAsync(CreateOrderModel request, string accUsername, string ipAddress)
        {
            try
            {

                if (request == null)
                {
                    return ApiResponse<string>.Error(null, "Request must not be null", 400);
                }

                if (request.Items == null || !request.Items.Any())
                {
                    return ApiResponse<string>.Error(null, "Order must have at least one item", 400);
                }

                if(request.PaymentMethod != "COD" && request.PaymentMethod != "ONLINE")
                {
                    return ApiResponse<string>.Error(null, "Invalid payment method. Only 'COD' and 'ONLINE' are supported.", 400);
                }

                string paymentUrl = null;

                var context = new SagaContext();

                var orchestrator = new SagaOrchestrator()

              .AddStep(new CreateOrderStep(_orderRepo, _productHttpClient, request, accUsername))
              .AddStep(new ApplyVoucherDetailStep(_voucherHttpClient, _orderRepo))
              .AddStep(new CreateTransactionStep(_transactionRepo))
;

                try
                {
                    await orchestrator.ExecuteAsync(context);
                }
                catch (InvalidOperationException ex)
                {
                    return ApiResponse<string>.Error(null, $"{ex.Message}", 400);
                }

                if (request.PaymentMethod == "ONLINE")
                {
                    var order = await _orderRepo.GetByIdAsync(context.OrderId);
                    if (order == null)
                    {
                        return ApiResponse<string>.Error(null, "Order not found", 500);
                    }
                    var orderInfo = $"Thanh toan don hang {order.OrderNumber}";
                    var orderNumber = order.OrderNumber;
                    var amount = order.TotalAmount;

                    var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
         ? "SE Asia Standard Time"
         : "Asia/Ho_Chi_Minh";

                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

                    var defaultExpire = timeNow.AddMinutes(10);
                   


                    var pay = new VnPayLibrary();

                    var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

                    pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                    pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                    pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
                    pay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString()); pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                    pay.AddRequestData("vnp_ExpireDate", defaultExpire.ToString("yyyyMMddHHmmss"));
                    pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
                    pay.AddRequestData("vnp_IpAddr", ipAddress);
                    pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
                    pay.AddRequestData("vnp_OrderInfo", orderNumber);
                    pay.AddRequestData("vnp_OrderType", "online");
                    pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
                    pay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

                     paymentUrl =
               pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);
                    //paymentUrl = await _vnpayService.CreatePaymentUrl(order.OrderNumber, amount, orderInfo, ipAddress);
                }

                return ApiResponse<string>.Ok(paymentUrl, "Order created successfully", 201); 
            }
            catch(Exception ex)
            {
                return ApiResponse<string>.Error(null, ex.Message, 500);
            }
        }

        //public async Task<ApiResponse<OrderDTO>> ProcessVnpayIpnAsync(string orderNumber, bool paymentSuccess, string vnpTransactionNo)
        //{
        //    try
        //    {
        //        var order = await _context.Orders
        //            .Include(o => o.OrderDetails)
        //            .Include(o => o.Transactions)
        //            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        //        if (order == null)
        //            return ApiResponse<OrderDTO>.Error(null, $"Order '{orderNumber}' not found", 404);

        //        var transaction = order.Transactions
        //            .FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");

        //        if (transaction == null)
        //            return ApiResponse<OrderDTO>.Error(null, "No pending payment transaction found", 400);

        //        if (paymentSuccess)
        //        {
        //            transaction.Status = "SUCCESS";
        //            order.Status = "CONFIRMED";
        //            order.ConfirmedDate = DateTime.UtcNow;
        //        }
        //        else
        //        {
        //            transaction.Status = "FAILED";
        //            order.Status = "CANCELLED";
        //            order.CancelReason = $"Payment failed — VNPAY txn: {vnpTransactionNo}";
        //            order.CancelledDate = DateTime.UtcNow;
        //        }

        //        order.UpdatedDate = DateTime.UtcNow;
        //        await _context.SaveChangesAsync();

        //        return ApiResponse<OrderDTO>.Ok(MapToDTO(order), paymentSuccess ? "Payment successful" : "Payment failed", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
        //    }
        //}

        public async Task<ApiResponse<OrderDTO>> GetOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepo.GetByIdAsync(orderId);

                if (order == null)
                    return ApiResponse<OrderDTO>.Error(null, $"Order {orderId} not found", 400);

                return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "OK", 200);
            }
            catch (Exception ex)
            {
                return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
            }
        }

        //public async Task<ApiResponse<OrderDTO>> GetOrderByNumberAsync(string orderNumber)
        //{
        //    try
        //    {
        //        var order = await _context.Orders
        //            .Include(o => o.OrderDetails)
        //            .Include(o => o.Transactions)
        //            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        //        if (order == null)
        //            return ApiResponse<OrderDTO>.Error(null, $"Order '{orderNumber}' not found", 404);

        //        return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "OK", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
        //    }
        //}

        //public async Task<ApiResponse<OrderDTO>> ConfirmPaymentAsync(int orderId)
        //{
        //    try
        //    {
        //        var order = await _context.Orders
        //            .Include(o => o.OrderDetails)
        //            .Include(o => o.Transactions)
        //            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        //        if (order == null)
        //            return ApiResponse<OrderDTO>.Error(null, $"Order {orderId} not found", 404);

        //        var transaction = order.Transactions
        //            .FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");

        //        if (transaction == null)
        //            return ApiResponse<OrderDTO>.Error(null, "No pending payment transaction found for this order", 400);

        //        if (order.Status != "PENDING")
        //            return ApiResponse<OrderDTO>.Error(null, $"Order is already in status '{order.Status}'", 400);

        //        transaction.Status = "SUCCESS";
        //        order.Status = "CONFIRMED";
        //        order.ConfirmedDate = DateTime.UtcNow;
        //        order.UpdatedDate = DateTime.UtcNow;

        //        await _context.SaveChangesAsync();

        //        return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "Payment confirmed successfully", 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return ApiResponse<OrderDTO>.Error(null, ex.Message, 500);
        //    }
        //}

        private OrderDTO MapToDTO(Order order)
        {
            var paymentTx = order.Transactions
                .FirstOrDefault(t => t.Type == "PAYMENT");

            return new OrderDTO
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Status = order.Status,
                ShippingAddress = order.ShippingAddress,
                PaymentMethod = order.PaymentMethod,
                
                TotalAmount = order.TotalAmount,
                ShippingPhone = order.ShippingPhone,
                ShippingName = order.ShippingName,
                ShippingFee = order.ShippingFee,
                DiscountAmount = order.DiscountAmount,
                Subtotal = order.Subtotal,

                CreatedDate = order.CreatedDate.Value,
                TransactionStatus = paymentTx?.Status,
                Items = order.OrderDetails.Select(d => new OrderDetailDTO
                {
                    OrderDetailId = d.OrderDetailId,
                    DiscountPerItem = d.DiscountPerItem.Value,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    
                    Subtotal = d.Subtotal,
                }).ToList()
            };
        }

        public async Task<ApiResponse<List<OrderDTO>>> GetOrdersByUsernameAsync(string accUsername)
        {
            var orders = await _orderRepo.GetByUsernameAsync(accUsername);

            var result = orders
                .Select(MapToDTO)
                .ToList();

            return ApiResponse<List<OrderDTO>>.Ok(
                result,
                "Get orders by username successfully",
                200
            );
        }

        public async Task<ApiResponse<List<OrderDTO>>> GetOrders()
        {
            var orders = await _orderRepo.GetAllOrders();

            var result = orders
                .Select(MapToDTO)
                .ToList();

            return ApiResponse<List<OrderDTO>>.Ok(
                result,
                "Get orders successfully",
                200
            );
        }

        public async Task<ApiResponse<OrderDTO>> UpdateOrderAsync(UpdateOrderModel request)
        {
            var order = await _orderRepo.GetByIdAsync(request.OrderId);

            if(order == null)
            {
                return ApiResponse<OrderDTO>.Error(null, $"Order {request.OrderId} not found", 404);
            }
            switch(request.Action)
            {
                case OrderAction.Confirm:
                    
                    order.ShippingName = request.ShippingName ?? order.ShippingName;
                    order.ShippingPhone = request.ShippingPhone ?? order.ShippingPhone;
                    order.ShippingAddress = request.ShippingAddress ?? order.ShippingAddress;

                    var paymentTx = order.Transactions
                                     .FirstOrDefault(t => t.Type == "PAYMENT");

                    bool isOnlinePaidSuccess =
       string.Equals(order.PaymentMethod, "ONLINE", StringComparison.OrdinalIgnoreCase)
       && string.Equals(paymentTx?.Status, "SUCCESS", StringComparison.OrdinalIgnoreCase);


                    bool isCod =
                         string.Equals(order.PaymentMethod, "COD", StringComparison.OrdinalIgnoreCase);

                    decimal totalRefundAmount = 0m;

                    foreach (var detail in order.OrderDetails)
                    {
                        var reqItem = request.OrderDetails.FirstOrDefault(x => x.ProductId == detail.ProductId);

                        if (reqItem == null)
                        {
                            return ApiResponse<OrderDTO>.Error(
                                null,
                                $"Missing item update for product {detail.ProductId}",
                                400
                            );
                        }


                        int orderedQty = detail.Quantity;
                        int providedQty = reqItem.Quantity;
                        int refundAmount = reqItem.RefundAmount;

                        if (providedQty < 0)
                        {
                            return ApiResponse<OrderDTO>.Error(
                                null,
                                $"Provided quantity of product {detail.ProductId} must be >= 0",
                                400
                            );
                        }
                        if (providedQty > orderedQty)
                        {
                            return ApiResponse<OrderDTO>.Error(
                                null,
                                $"Provided quantity of product {detail.ProductId} cannot exceed ordered quantity",
                                400
                            );
                        }
                        int missingQty = orderedQty - providedQty;
                        if (isCod)
                        {
                            detail.MissingQuantity = missingQty;
                            detail.RefundQuantity = 0;
                            detail.RefundAmount = 0;

                            // cập nhật số lượng thực cấp
                            detail.Quantity = providedQty;
                            detail.Subtotal = (detail.Price ?? 0) * providedQty - ((detail.DiscountPerItem ?? 0) * providedQty);

                            continue;
                        }

                        if(!isOnlinePaidSuccess)
                        {
                            return ApiResponse<OrderDTO>.Error(
                                null,
                                $"Cannot confirm order because online payment is not successful.",
                                400
                            );
                        }

                        if (refundAmount < 0)
                        {
                            return ApiResponse<OrderDTO>.Error(
                                null,
                                $"Refund amount of product {detail.ProductId} must be >= 0",
                                400
                            );
                        }
                      

                        totalRefundAmount += refundAmount;

                        detail.MissingQuantity = missingQty;
                        detail.RefundQuantity = missingQty;
                        detail.RefundAmount = refundAmount;

                        // cập nhật số lượng thực cấp
                        detail.Quantity = providedQty;
                        detail.Subtotal = detail.Price.Value * providedQty - refundAmount;

                        decimal originalTotal = order.TotalAmount;
                        if (totalRefundAmount > originalTotal)
                        {
                            return ApiResponse<OrderDTO>.Error(
                                null,
                                "Total refund amount cannot exceed total order amount",
                                400
                            );
                        }
                        order.TotalAmount = originalTotal - totalRefundAmount;
                    }
                    order.Status = "PACKAGING";

                    if (totalRefundAmount > 0)
                    {

                        var transaction = new Transaction
                        {
                            OrderId = order.OrderId,
                            Direction = "OUT",
                            Type = "REFUND",
                            Amount = totalRefundAmount,
                            Status = "PENDING",
                        };

                        await _transactionRepo.CreateAsync(transaction);
                    }

                    break;
                case OrderAction.Delivery:
                    if (order.Status != "PACKAGING")
                    {
                        return ApiResponse<OrderDTO>.Error(
                            null,
                            $"Cannot confirm order because current status is {order.Status}. Only PENDING order can be confirmed.",
                            400
                        );
                    }
                    order.Status = "DELIVERING";
                    break;
                case OrderAction.Complete:
                    if (order.Status != "DELIVERING")
                    {
                        return ApiResponse<OrderDTO>.Error(
                            null,
                            $"Cannot complete order because current status is {order.Status}. Only DELIVERING order can be confirmed.",
                            400
                        );
                    }
                    order.Status = "COMPLETED";
                    break;
                default:
                    return ApiResponse<OrderDTO>.Error(null, "Invalid action", 400);
            }

            await _orderRepo.UpdateAsync(order);

            return ApiResponse<OrderDTO>.Ok(MapToDTO(order), "Update order successfully");

        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(CancelOrderRequest request)
        {

            if(request == null)
            {
                return ApiResponse<bool>.Error(false, "Request must not be null", 400);
            }

            if(request.OrderId <= 0)
            {
                return ApiResponse<bool>.Error(false, "Invalid order id", 400);
            }

            var context = new SagaContext();

            var orchestrator = new SagaOrchestrator()
                .AddStep(new CancelOrderStep(request.OrderId, _orderRepo, request.CancelReason))
                .AddStep(new UnapplyVoucherDetailStep(_voucherHttpClient))
                .AddStep(new CreateRefundTransactionStep(_transactionRepo));

            try
            {
                await orchestrator.ExecuteAsync(context);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponse<bool>.Error(false, $"{ex.Message}", 400);
            } catch(Exception ex)
            {
                return ApiResponse<bool>.Error(false, ex.Message, 500);
             }
             return ApiResponse<bool>.Ok(true, "Order cancelled successfully", 200);
        }

        public async Task<ApiResponse<bool>> ProcessVnPay(string vnp_TxnRef, string vnp_ResponseCode)
        {

            if (!int.TryParse(vnp_TxnRef, out var orderId))
            {
                return ApiResponse<bool>.Error(false, "Invalid order id", 400);
            }

            var order = await _orderRepo.GetByIdAsync(orderId);

            if(order == null)
            {
                return ApiResponse<bool>.Error(false, $"Order {orderId} not found", 404);
            }

            var transaction = order.Transactions.FirstOrDefault(t => t.Type == "PAYMENT" && t.Status == "PENDING");

            if (vnp_ResponseCode == "00")
            {

                transaction.Status = "SUCCESS";

                await _transactionRepo.UpdateAsync(transaction);

            } else
            {
                transaction.Status = "FAILED";
                order.Status = "CANCELLED";
                order.CancelReason = $"Payment failed — VNPAY txn: {vnp_TxnRef}";
                await _orderRepo.UpdateAsync(order);
                await _transactionRepo.UpdateAsync(transaction);
            }

            return ApiResponse<bool>.Ok(true, $"Processed VNPAY callback for order {vnp_TxnRef}", 200);
        }
    }
}

