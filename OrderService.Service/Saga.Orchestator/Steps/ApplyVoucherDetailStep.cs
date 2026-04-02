using OrderService.Repository;
using OrderService.Service.DTO.External.Request;
using OrderService.Service.HttpClients;
using OrderService.Service.Saga.Orchestator.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    public class ApplyVoucherDetailStep : ISagaStep
    {
        private readonly IVoucherHttpClient _voucherHttpClient;
        private readonly IOrderRepo _orderRepo;

        public ApplyVoucherDetailStep(IVoucherHttpClient voucherHttpClient, IOrderRepo orderRepo)
        {
            _voucherHttpClient = voucherHttpClient;
            _orderRepo = orderRepo;
        }

        public Task CompensateAsync(SagaContext sagaContext)
        {
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {

            if (sagaContext.VoucherIds.Count == 0)
            {
                return;
            }

            CreateVoucherDetailRequest request = new CreateVoucherDetailRequest
            {
                OrderId = sagaContext.OrderId,
                VoucherIds = sagaContext.VoucherIds,
                totalAmountOrder = sagaContext.TotalAmountOrder.Value
            };

            var items = await _voucherHttpClient.ApplyVoucherDetailAsync(request);

            decimal totalDiscount = items.Sum(x => x.DiscountAmount);

            var order = await _orderRepo.GetByIdAsync(sagaContext.OrderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            order.DiscountAmount = totalDiscount;
            order.TotalAmount = order.TotalAmount - (order.DiscountAmount ?? 0);

            await _orderRepo.UpdateAsync(order);

            sagaContext.TotalAmountOrder = order.TotalAmount;

        }
    }
}
