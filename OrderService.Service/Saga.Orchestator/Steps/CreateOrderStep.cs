using OrderService.Model.Entities;
using OrderService.Repository;
using OrderService.Service.DTO.Request;
using OrderService.Service.HttpClients;
using OrderService.Service.Saga.Orchestator.Context;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    public class CreateOrderStep : ISagaStep
    {
        private readonly CreateOrderModel _request;
        private readonly string _accountUsername;
        private readonly IOrderRepo _orderRepo;
        private readonly IProductHttpClient _productHttpClient;


        public CreateOrderStep( IOrderRepo orderRepo, IProductHttpClient productHttpClient, CreateOrderModel request, string accountUsername)
        {
            _request = request;
            _orderRepo = orderRepo;
            _productHttpClient = productHttpClient;
            _accountUsername = accountUsername;
        }

        public async Task CompensateAsync(SagaContext sagaContext)
        {

            var order = await _orderRepo.GetByIdAsync(sagaContext.OrderId);

            if(order == null)
            {
                return;
            }

            await _orderRepo.DeleteAsync(order);

        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            var order = new Order
            {
                AccountUsername = _accountUsername,
                OrderNumber = "ORD-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                Status = "PENDING",
                ShippingName = _request.ShippingName,
                ShippingPhone = _request.ShippingPhone,
                ShippingAddress = _request.ShippingAddress,
                PaymentMethod = _request.PaymentMethod,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,

                OrderDetails = new List<OrderDetail>()
            };

            decimal subTotal = 0;

            for (int i = 0; i < _request.Items.Count; i++)
            {
                var product = await _productHttpClient.GetProductByIdAsync(_request.Items[i].ProductId);
                if (product == null)

                {
                    throw new InvalidOperationException($"Product with ID {_request.Items[i].ProductId} not found.");
                }
                var unitPrice = product.PriceSell - _request.Items[i].Discount; 
                var lineTotal = unitPrice * _request.Items[i].Quantity;
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = _request.Items[i].ProductId,
                    Quantity = _request.Items[i].Quantity,
                    DiscountPerItem = _request.Items[i].Discount,
                    Subtotal = lineTotal,
                    Price = product.PriceSell,
                    ProductName = product.ProductName,
                    CreatedAt = DateTime.UtcNow
                });
                subTotal += lineTotal;
            }

            order.Subtotal = subTotal;
            order.DiscountAmount = 0;
            order.ShippingFee = _request.ShippingFee;
            order.TotalAmount = subTotal + _request.ShippingFee.Value;

            var created = await _orderRepo.CreateAsync(order);

            if (created == null)
            {
                throw new InvalidOperationException("Failed to create order.");
            }
            sagaContext.OrderId = created.OrderId;
            sagaContext.VoucherIds = _request.VoucherIds;
            sagaContext.TotalAmountOrder = created.TotalAmount;
        }
    }
}
