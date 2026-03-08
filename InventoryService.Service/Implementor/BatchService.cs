using InventoryService.Model;
using InventoryService.Repository;
using InventoryService.Service.DTO;
using InventoryService.Service.DTO.Request;
using InventoryService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service.Implementors
{
    public class BatchService : IBatchService
    {
        IBatchRepo _repo;
       
        public BatchService(IBatchRepo repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<BatchDTO>> AddBatchAsync(CreateBatchModel request)
        {
            try
            {
            
                var count = await _repo.CountBatchBySupplierAsync(request.SupplierId);
                var orderNumber = count + 1;

                var batchCode = $"BA-{request.SupplierId}-{orderNumber}";
                var batch = new Batch
                {
                    SupplierId = request.SupplierId,
                    BatchCode = batchCode,
                    TotalItems = request.Items.Count(),
                    Status = "PENDING",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    BatchDetails = request.Items.Select(i => new BatchDetail
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity
                    }).ToList()
                };

                var created = await _repo.AddBatchAsync(batch);

                return ApiResponse<BatchDTO>.Ok(null, "Batch added successfully", 201);
            } catch (Exception ex)
            {
                return ApiResponse<BatchDTO>.Error(null, $"Error adding batch: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<BatchDTO>>> GetAllBatchesAsync()
        {
            try
            {
                var list = await _repo.GetAllBatchesAsync();

                var batchDTOs = list.Select(b => new BatchDTO
                {
                    BatchId = b.BatchId,
                    SupplierName = b.Supplier.Name,
                    SupplierPhone = b.Supplier.Phone,
                    SupplierAddress = b.Supplier.Address,
                    CreatedBy = b.CreatedBy,
                    BatchCode = b.BatchCode,
                    TotalItems = b.TotalItems,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    DeliveredDate = b.DeliveredDate,
                    ImageConfirmReceived = b.ImageConfirmReceived,
                    Notes = b.Notes,
                    CreatedDate = b.CreatedDate,
                    UpdatedDate = b.UpdatedDate
                }).ToList();

                return ApiResponse<List<BatchDTO>>.Ok(batchDTOs, "Batches retrieved successfully", 200);

            }
            catch (Exception ex)
            {
                return ApiResponse<List<BatchDTO>>.Error(null, $"Error retrieving batches: {ex.Message}", 500);
            }
        }

public async Task<ApiResponse<BatchDTO>> GetBatchByIdAsync(int id)
        {
            var batch = await _repo.GetBatchByIdAsync(id);
            if (batch == null)
            {
                return ApiResponse<BatchDTO>.Error(null, "Batch not found", 404);
            }
            var batchDTO = new BatchDTO
            {
                BatchId = batch.BatchId,
                SupplierName = batch.Supplier.Name,
                SupplierPhone = batch.Supplier.Phone,
                SupplierAddress = batch.Supplier.Address,
                CreatedBy = batch.CreatedBy,
                BatchCode = batch.BatchCode,
                TotalItems = batch.TotalItems,
                TotalPrice = batch.TotalPrice,
                Status = batch.Status,
                DeliveredDate = batch.DeliveredDate,
                ImageConfirmReceived = batch.ImageConfirmReceived,
                Notes = batch.Notes,
                CreatedDate = batch.CreatedDate,
                UpdatedDate = batch.UpdatedDate,
                BatchDetails = batch.BatchDetails.Select(d => new BatchDeatailDTO
                {
                    BatchDetailId = d.BatchDetailId,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    Subtotal = d.Subtotal,
                }).ToList()
            };
            return ApiResponse<BatchDTO>.Ok(batchDTO, "Batch retrieved successfully", 200);
        }

        public Task<ApiResponse<BatchDTO>> UpdateBatchAsync(Batch batch)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<BatchDTO>> UpdateBatchAsync(CreateBatchModel request)
        {
            throw new NotImplementedException();
        }
    }
}
