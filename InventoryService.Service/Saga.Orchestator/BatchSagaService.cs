using InventoryService.Repository;
using InventoryService.Service.DTO;
using InventoryService.Service.DTO.Request;
using InventoryService.Service.Saga.Orchestator.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service.Saga.Orchestator
{
    public class BatchSagaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBatchDetailRepo _batchDetailRepo;

        public BatchSagaService(IHttpClientFactory httpClientFactory, IBatchDetailRepo batchDetailRepo)
        {
            _httpClientFactory = httpClientFactory;
            _batchDetailRepo = batchDetailRepo;
        }
        public async Task<string> ProcessBatch(List<UpdateItem> items)
        {

            var successItems = new List<UpdateItem>();

            foreach (var item in items)
            {
                var updatedItems = new List<(int ProductId, int Quantity)>();

                var batchDetail = await _batchDetailRepo.GetBatchDetailById(item.Id);

                if (batchDetail == null)
                {
                    return $"Batch Detail not found";
                }

                var response = await UpdateInventory(batchDetail.ProductId, item.Quantity);

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (apiResponse == null || !apiResponse.Success)
                {
                    // rollback
                    foreach (var success in updatedItems)
                    {
                        await RollbackInventory(success.ProductId, success.Quantity);
                    }

                    return apiResponse.Message;
                }

                updatedItems.Add((batchDetail.ProductId, item.Quantity));
            }

            return "";
        }

        public async Task<HttpResponseMessage> UpdateInventory(int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient();

            var request = new UpdateProductQtyRequest
            {
                ProductId = productId,
                ProductQty = quantity,
                Action = UpdateQtyAction.Add
            };

            return await client.PutAsJsonAsync(
                "http://productcatalogservice.api:7000/api/product/quantity",
                request
            );
        }
             public async Task<HttpResponseMessage> RollbackInventory(int productId, int quantity)
        {
            var client = _httpClientFactory.CreateClient();

            var request = new UpdateProductQtyRequest
            {
                ProductId = productId,
                ProductQty = quantity,
                Action = UpdateQtyAction.Subtract
            };

            return await client.PutAsJsonAsync(
                "http://productcatalogservice.api:7000/api/product/quantity",
                request);
        }
    }

    
}
