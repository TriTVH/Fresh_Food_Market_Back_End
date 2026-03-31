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
        
        public ApplyVoucherDetailStep() 
        {
        
        }

        public async Task CompensateAsync(SagaContext sagaContext)
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteAsync(SagaContext sagaContext)
        {
            throw new NotImplementedException();
        }
    }
}
