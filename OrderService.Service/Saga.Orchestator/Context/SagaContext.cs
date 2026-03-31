using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator.Context
{
    public class SagaContext
    {
       
            public int OrderId { get; set; }
            public List<int> VoucherIds { get; set; } = new();
       
    }
}
