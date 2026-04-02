using OrderService.Service.HttpClients;
using OrderService.Service.Saga.Orchestator.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator.Steps
{
    public class UnapplyVoucherDetailStep : ISagaStep
    {
        private readonly IVoucherHttpClient _voucherHttpClient;
        public UnapplyVoucherDetailStep(IVoucherHttpClient voucherHttpClient)
        {
            _voucherHttpClient = voucherHttpClient;
        }
        public Task CompensateAsync(SagaContext sagaContext)
        {
          return Task.CompletedTask;
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            var result = await _voucherHttpClient.UnApplyVoucherDetailAsync(sagaContext.CancelOrderId);
        }
    }
}
