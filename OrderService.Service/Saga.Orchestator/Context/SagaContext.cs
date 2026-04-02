namespace OrderService.Service.Saga.Orchestator.Context
{
    public class SagaContext
    {
            public int OrderId { get; set; }
            public List<int> VoucherIds { get; set; } = new();
            public decimal? TotalAmountOrder { get; set; }

            public int TransactionId { get; set; }


        public int CancelOrderId { get; set; }
        public string OrderStatus { get; set; }

        public decimal TotalAmountCancel { get; set; }

        public int TransactionCancelId { get; set; }
    }
}
