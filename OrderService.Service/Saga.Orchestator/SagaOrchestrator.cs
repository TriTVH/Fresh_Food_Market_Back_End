using OrderService.Service.Saga.Orchestator.Context;
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

        public async Task ExecuteAsync(SagaContext sagaContext)
        {

            _executed.Clear();

            foreach (var step in _steps)
            {
                try
                {
                    await step.ExecuteAsync(sagaContext);
                    _executed.Add(step);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Step {step.GetType().Name} failed: {ex.Message}");

                    await CompensateAsync(sagaContext);

                    throw;
                }
            }
        }

        private async Task CompensateAsync(SagaContext sagaContext)
        {
            foreach (var step in _executed.AsEnumerable().Reverse())
            {
                try
                {
                    await step.CompensateAsync(sagaContext);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Compensation failed: {ex.Message}");
                }
            }
        }
    }
}
