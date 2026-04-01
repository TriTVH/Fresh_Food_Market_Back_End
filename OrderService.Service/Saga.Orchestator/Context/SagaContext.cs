namespace OrderService.Service.Saga.Orchestator.Context
{
    public class SagaContext
    {
            public int OrderId { get; set; }
            public List<int> VoucherIds { get; set; } = new();
            public decimal? TotalAmountOrder { get; set; }
    }
}
