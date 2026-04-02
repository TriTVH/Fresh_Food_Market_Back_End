using OrderService.Model.Entities;
using OrderService.Repository;
using OrderService.Service.Saga.Orchestator.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    public class CreateRefundTransactionStep : ISagaStep
    {
        private readonly ITransactionRepo _transactionRepo;
        public CreateRefundTransactionStep(ITransactionRepo transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }
        public async Task CompensateAsync(SagaContext sagaContext)
        {
            var transaction = await _transactionRepo.GetByIdAsync(sagaContext.TransactionCancelId);

            if (transaction == null)
            {
                return;
            }
            
            await _transactionRepo.DeleteAsync(transaction);
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            var transaction = new Transaction
            {
                Type = "REFUND",
                Direction = "OUT",
                Status = "PENDING",
                Amount = sagaContext.TotalAmountCancel,
                OrderId = sagaContext.CancelOrderId
            };
            var created = await _transactionRepo.CreateAsync(transaction);

            sagaContext.TransactionCancelId = created.Id;
        }
    }
}
