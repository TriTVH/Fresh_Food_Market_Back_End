using OrderService.Model.Entities;
using OrderService.Repository;
using OrderService.Service.Saga.Orchestator.Context;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    /// <summary>
    /// Tạo bản ghi transaction PENDING cho đơn hàng.
    /// type    = PAYMENT
    /// direction = IN  (tiền chạy vào hệ thống từ khách hàng)
    /// status  = PENDING → SUCCESS sau khi xác nhận thanh toán
    /// </summary>
    public class CreateTransactionStep : ISagaStep
    {
        private readonly ITransactionRepo _transactionRepo;

        public CreateTransactionStep(ITransactionRepo transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            var transaction = new Transaction
            {
                Type = "PAYMENT",
                Direction = "IN",
                Amount = sagaContext.TotalAmount,
                Status = "PENDING",
                OrderId = sagaContext.OrderId
            };

            var created = await _transactionRepo.CreateAsync(transaction);
            sagaContext.TransactionId = created.Id;
        }

        public async Task CompensateAsync(SagaContext sagaContext)
        {
            if (sagaContext.TransactionId > 0)
            {
                var transaction = await _transactionRepo.GetByIdAsync(sagaContext.TransactionId);
                if (transaction != null)
                {
                    transaction.Status = "CANCELLED";
                    await _transactionRepo.UpdateAsync(transaction);
                }
            }
        }
    }
}
