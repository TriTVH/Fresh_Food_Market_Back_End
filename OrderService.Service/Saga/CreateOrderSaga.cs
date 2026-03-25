using OrderService.Model;
using OrderService.Repository;
using OrderService.Service.DTO.Request;
using OrderService.Service.DTO.Response;
using OrderService.Service.HttpClients;

namespace OrderService.Service.Saga
{
    public class CreateOrderSaga
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IProductHttpClient _productHttpClient;

        // Tracks what was persisted so we can compensate on failure
        private Order? _createdOrder;

        public CreateOrderSaga(IOrderRepo orderRepo, IProductHttpClient productHttpClient)
        {
            _orderRepo = orderRepo;
            _productHttpClient = productHttpClient;
        }

        public async Task<OrderDTO> ExecuteAsync(CreateOrderModel request)
        {
            // Step 1: Validate all products via ProductCatalogService HTTP call
            var resolvedItems = new List<(CreateOrderDetailModel item, DTO.External.ProductDTO product)>();

            foreach (var item in request.Items)
            {
                var product = await _productHttpClient.GetProductByIdAsync(item.ProductId);

                if (product == null)
                    throw new InvalidOperationException($"Product {item.ProductId} not found");

                if (product.IsAvailable != true)
                    throw new InvalidOperationException($"Product {item.ProductId} is not available");

                resolvedItems.Add((item, product));
            }

            // Step 2: Build and persist order with status = pending
            try
            {
                var subtotal = resolvedItems.Sum(x => x.product.PriceSell * x.item.Quantity);
                var shippingFee = request.ShippingFee ?? 0;

                var order = new Order
                {
                    AccountId = request.AccountId,
                    SubscriptionId = request.SubscriptionId,
                    VoucherId = request.VoucherId,
                    OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
                    OrderDate = DateTime.UtcNow,
                    Status = "pending",
                    ShippingName = request.ShippingName,
                    ShippingPhone = request.ShippingPhone,
                    ShippingAddress = request.ShippingAddress,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "unpaid",
                    Subtotal = subtotal,
                    DiscountAmount = 0,
                    ShippingFee = shippingFee,
                    TotalAmount = subtotal + shippingFee,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    OrderDetails = resolvedItems.Select(x => new OrderDetail
                    {
                        ProductId = x.item.ProductId,
                        ProductName = x.product.ProductName,
                        Quantity = x.item.Quantity,
                        Price = (int)x.product.PriceSell,
                        DiscountPerItem = 0,
                        Subtotal = x.product.PriceSell * x.item.Quantity,
                        CreatedAt = DateTime.UtcNow
                    }).ToList()
                };

                _createdOrder = await _orderRepo.CreateAsync(order);
                return MapToDTO(_createdOrder);
            }
            catch
            {
                // Compensation: mark order cancelled if it was already written
                await CompensateAsync();
                throw;
            }
        }

        private async Task CompensateAsync()
        {
            if (_createdOrder != null)
            {
                _createdOrder.Status = "cancelled";
                _createdOrder.CancelReason = "Saga compensation — order creation failed";
                _createdOrder.CancelledDate = DateTime.UtcNow;
                _createdOrder.UpdatedDate = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(_createdOrder);
            }
        }

        private static OrderDTO MapToDTO(Order order) => new OrderDTO
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            Items = order.OrderDetails.Select(d => new OrderDetailDTO
            {
                OrderDetailId = d.OrderDetailId,
                ProductId = d.ProductId,
                ProductName = d.ProductName,
                Quantity = d.Quantity,
                Price = d.Price,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }
}
