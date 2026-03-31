using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.Saga.Orchestator
{
    public class SagaOrchestrator
    {
        private readonly List<ISagaStep> _steps = new();
        private readonly List<ISagaStep> _executed = new();
        public SagaOrchestrator AddStep(ISagaStep step)
        {
            _steps.Add(step);
            return this;
        }

        public async Task ExecuteAsync()
        {
            foreach (var step in _steps)
            {
                try
                {
                    await step.ExecuteAsync();
                    _executed.Add(step);
                }
                catch (Exception ex)
                {
                    await CompensateAsync();
                    throw;
                }
            }
        }

        private async Task CompensateAsync()
        {
            foreach (var step in _executed.AsEnumerable().Reverse())
            {
                try 
                { 
                    await step.CompensateAsync(); 
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine($"⚠️ Compensation failed: {ex.Message}"); 
                }
            }
        }
    }
}
