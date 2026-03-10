using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service.Saga.Orchestator
{
    public class BatchSagaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BatchSagaService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

    }
}
