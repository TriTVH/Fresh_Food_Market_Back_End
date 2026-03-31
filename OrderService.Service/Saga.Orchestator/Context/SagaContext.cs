namespace OrderService.Service.Saga.Orchestator.Context
{
    public class SagaContext
    {
        public int OrderId { get; set; }
        public int TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<int> VoucherIds { get; set; } = new();
    }
}
