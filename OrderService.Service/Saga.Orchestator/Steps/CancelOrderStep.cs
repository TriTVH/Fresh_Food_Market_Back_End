using OrderService.Repository;
using OrderService.Service.Saga.Orchestator.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    public class CancelOrderStep : ISagaStep
    {
        private int _orderId;
        private string _cancelReason;
        private readonly IOrderRepo _orderRepository;

        public CancelOrderStep(int orderId, IOrderRepo orderRepo, string cancelReason)
        {
            _orderId = orderId;
            _orderRepository = orderRepo;
            _cancelReason = cancelReason;
        }

        public async Task CompensateAsync(SagaContext sagaContext)
        {
            var order = await _orderRepository.GetByIdAsync(_orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {_orderId} not found.");
            }
            order.Status = sagaContext.OrderStatus;
            order.CancelledDate = null;
            order.CancelReason = null;
            await _orderRepository.UpdateAsync(order);
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            var order = await _orderRepository.GetByIdAsync(_orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with ID {_orderId} not found.");
            }
            order.Status = "CANCELLED";
            order.CancelledDate = DateTime.UtcNow;
            order.CancelReason = _cancelReason;
            await _orderRepository.UpdateAsync(order);

            sagaContext.CancelOrderId = order.OrderId;
            sagaContext.OrderStatus = order.Status;
            sagaContext.TotalAmountCancel = order.TotalAmount;
        }
    }
}
