using OrderService.Service.Saga.Orchestator.Context;

namespace OrderService.Service.Saga.Orchestator
{
    public class SagaOrchestrator
    {
        private readonly List<ISagaStep> _steps = new();
        private readonly List<ISagaStep> _executed = new();
        private readonly SagaContext _context = new();

        public SagaOrchestrator AddStep(ISagaStep step)
        {
            _steps.Add(step);
            return this;
        }

        public async Task<SagaContext> ExecuteAsync()
        {
            foreach (var step in _steps)
            {
                try
                {
                    await step.ExecuteAsync(_context);
                    _executed.Add(step);
                }
                catch
                {
                    await CompensateAsync();
                    throw;
                }
            }
            return _context;
        }

        private async Task CompensateAsync()
        {
            foreach (var step in _executed.AsEnumerable().Reverse())
            {
                try
                {
                    await step.CompensateAsync(_context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Compensation failed: {ex.Message}");
                }
            }
        }
    }
}
