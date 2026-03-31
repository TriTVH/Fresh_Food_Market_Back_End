using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator
{
    public interface ISagaStep
    {
        Task ExecuteAsync();
        Task CompensateAsync();
    }
}
