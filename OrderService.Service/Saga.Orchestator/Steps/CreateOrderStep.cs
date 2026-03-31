using OrderService.Model.Entities;
using OrderService.Repository;
using OrderService.Service.DTO.Request;
using OrderService.Service.HttpClients;
using OrderService.Service.Saga.Orchestator.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    public class CreateOrderStep : ISagaStep
    {
        private readonly string _accountUsername;
        private readonly CreateOrderModel _request;
        private readonly IOrderRepo _orderRepo;
        private readonly IProductHttpClient _productHttpClient;

        public CreateOrderStep(CreateOrderModel request, IOrderRepo orderRepo, IProductHttpClient productHttpClient,string accountUsername)
        {
            _request = request;
            _orderRepo = orderRepo;
            _productHttpClient = productHttpClient;
        }

        public Task CompensateAsync(SagaContext sagaContext)
        {
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            var order = new Order
            {
                AccountUsername = _accountUsername,
                OrderNumber = "ORD-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                OrderDate = DateTime.UtcNow,
                Status = "PENDING",
                ShippingName = _request.ShippingName,
                ShippingPhone = _request.ShippingPhone,
                ShippingAddress = _request.ShippingAddress,
                ShippingFee = _request.ShippingFee,
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
                    throw new Exception($"Product with ID {_request.Items[i].ProductId} not found.");
                }
                var unitPrice = product.PriceSell; // đổi lại đúng field giá bên product DTO của bạn
                var lineTotal = unitPrice * _request.Items[i].Quantity;
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = _request.Items[i].ProductId,
                    Quantity = _request.Items[i].Quantity,
                    DiscountPerItem = _request.Items[i].Discount,
                    Subtotal = lineTotal,
                    Price = unitPrice,
                    ProductName = product.ProductName,
                    CreatedAt = DateTime.UtcNow
                });
                subTotal += lineTotal;
            }
            order.Subtotal = subTotal;
            order.DiscountAmount = 0;
            order.TotalAmount = subTotal - (order.DiscountAmount ?? 0) + (order.ShippingFee ?? 0);
            var created = await _orderRepo.CreateAsync(order);

            if (created == null)
            {
                throw new Exception("Failed to create order.");
            }

            sagaContext.OrderId = created.OrderId;
            sagaContext.VoucherIds = _request.VoucherIds;
        }
    }
}
