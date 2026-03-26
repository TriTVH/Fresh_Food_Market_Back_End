using InventoryService.Model;
using InventoryService.Repository;
using InventoryService.Service.DTO;
using InventoryService.Service.DTO.Request;
using InventoryService.Service.DTO.Response;
using InventoryService.Service.Saga.Orchestator;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace InventoryService.Service.Implementors
{
    public class BatchService : IBatchService
    {
        IBatchRepo _repo;
        ISupplierRepo _supplierRepo;
        private readonly BatchSagaService _batchSagaService;
        private readonly IConnectionMultiplexer _redis;
        private readonly IHttpClientFactory _httpClientFactory;


        public BatchService(ISupplierRepo supplierRepo,IBatchRepo repo, IConnectionMultiplexer redis, IHttpClientFactory httpClientFactory, BatchSagaService batchSagaService   )
        {
            _repo = repo;
            _supplierRepo = supplierRepo;
            _redis = redis;
            _httpClientFactory = httpClientFactory;
            _batchSagaService = batchSagaService;
        }

        public async Task<ApiResponse<BatchDTO>> AddBatchAsync(CreateBatchModel request)
        {
            try
            {
            
                var count = await _repo.CountBatchBySupplierAsync(request.SupplierId);
                var orderNumber = count + 1;

                var batchCode = $"BA-{request.SupplierId}-{orderNumber}";

                var batchDetails = new List<BatchDetail>();

                if(await _supplierRepo.GetSupplierById(request.SupplierId) == null)
                {
                    return ApiResponse<BatchDTO>.Error(
                        null,
                        $"Supplier with Id: {request.SupplierId} doesn't exist",
                        400
                    );
                }

                foreach (var item in request.Items)
                {
                    var product = await GetProductByIdFromRedisAsync("products", item.ProductId);

                    if (product == null)
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            $"Product with Id: {item.ProductId} doesn't exist",
                            400
                        );
                    }

                    batchDetails.Add(new BatchDetail
                    {
                        ProductId = item.ProductId,
                        ProductName = product.ProductName,
                        Quantity = item.Quantity
                    });
                }


                var batch = new Batch
                {
                    SupplierId = request.SupplierId,
                    BatchCode = batchCode,
                    TotalItems = request.Items.Count(),
                    Status = "PENDING",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    BatchDetails = batchDetails
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
                    SupplyBy = b.CreatedBy,
                    BatchCode = b.BatchCode,
                    TotalItems = b.TotalItems,
                    Status = b.Status,
                    DeliveredDate = b.DeliveredDate,
                    CreatedDate = b.CreatedDate,
                    UpdatedDate = b.UpdatedDate,
                   
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
                SupplyBy = batch.CreatedBy,
                BatchCode = batch.BatchCode,
                TotalItems = batch.TotalItems,
                Status = batch.Status,
                DeliveredDate = batch.DeliveredDate,
                ImageConfirmReceived = DesializeImageItem(batch.ImageConfirmReceived),
                Notes = DeserializeBatchNote(batch.Notes),
                CreatedDate = batch.CreatedDate,
                UpdatedDate = batch.UpdatedDate,
               
                BatchDetails = batch.BatchDetails.Select(d => new BatchDetailDTO
                {
                    BatchDetailId = d.BatchDetailId,
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    ExpiredDate = d.ExpiredDate
                }).ToList()
            };
            return ApiResponse<BatchDTO>.Ok(batchDTO, "Batch retrieved successfully", 200);
        }

        private async Task<ProductDTO?> GetProductByIdFromRedisAsync(string redisKey, int productId)
        {
            var db = _redis.GetDatabase();

            if(!await db.KeyExistsAsync(redisKey))
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("http://productcatalogservice.api:7000/api/product");
            }

            var value = await db.HashGetAsync(redisKey, productId.ToString());

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<ProductDTO>(value!);
        }

        public async Task<ApiResponse<BatchDTO>> UpdateBatchAsync(UpdateBatchModel request, string username, string role )
        {
            var batch = await _repo.GetBatchByIdAsync(request.Id);
            if (batch == null)
            {
                return ApiResponse<BatchDTO>.Error(null, "Batch not found", 404);
            }
            var note = new BatchNoteModel();

            if (!HasPermission(request.Action, role))
            {
                return ApiResponse<BatchDTO>.Error(
                    null,
                    $"Forbidden: You are not allowed to perform action '{request.Action}'",
                    403
                );
            }
            var requestItemMap = request.Items.ToDictionary(x => x.Id, x => x);

            switch (request.Action)
            {
                case BatchAction.Confirm:
                    if (batch.Status != "PENDING")
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            $"Cannot confirm batch because current status is {batch.Status}. Only PENDING batch can be confirmed.",
                            400
                        );
                    }
                    if (!batch.BatchDetails.Any())
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            "Cannot confirm batch because batch has no details.",
                            400
                        );
                    }

                    note = DeserializeBatchNote(batch.Notes);

                    var insufficientSupplyNote = new List<MissingSupplyNote>();
                    var unprovidedProducts = new List<MissingSupplyNote>();


                    foreach (var detail in batch.BatchDetails)
                    {
                        if (!requestItemMap.TryGetValue(detail.BatchDetailId, out var item))
                        {
                            unprovidedProducts.Add(new MissingSupplyNote
                            {
                                BatchDetailId = detail.BatchDetailId,
                                ProductId = detail.ProductId,
                                ProductName = detail.ProductName ?? string.Empty,
                                Required = detail.Quantity,
                                Provided = 0,
                                Missing = detail.Quantity
                            });

                            continue;
                        }
                        if (item.Quantity < 0)
                        {
                            return ApiResponse<BatchDTO>.Error(
                                null,
                                $"Quantity cannot be negative for BatchDetailId {item.Id}.",
                                400
                            );
                        }
                        var requiredQuantity = detail.Quantity;
                        var providedQuantity = item.Quantity;

                        if (providedQuantity > requiredQuantity)
                        {
                            return ApiResponse<BatchDTO>.Error(
                                null,
                                $"Provided quantity for BatchDetailId {item.Id} cannot exceed required quantity of {requiredQuantity}.",
                                400
                            );
                        }

                        if (providedQuantity < requiredQuantity)
                        {
                            if (providedQuantity == 0)
                            {
                                unprovidedProducts.Add(new MissingSupplyNote
                                {
                                    BatchDetailId = detail.BatchDetailId,
                                    ProductId = detail.ProductId,
                                    ProductName = detail.ProductName ?? string.Empty,
                                    Required = requiredQuantity,
                                    Provided = providedQuantity,
                                    Missing = requiredQuantity - providedQuantity
                                });
                                detail.Quantity = providedQuantity;
                                detail.ExpiredDate = item.ExpiredDate;
                                continue;
                            }

                            insufficientSupplyNote.Add(new MissingSupplyNote
                            {
                                BatchDetailId = detail.BatchDetailId,
                                ProductId = detail.ProductId,
                                ProductName = detail.ProductName ?? string.Empty,
                                Required = requiredQuantity,
                                Provided = providedQuantity,
                                Missing = requiredQuantity - providedQuantity
                            });
                        }

                        detail.Quantity = providedQuantity;
                        detail.ExpiredDate = item.ExpiredDate;
                    }

                    note.InsufficientSupplyNote = insufficientSupplyNote;
                    note.UndeliverableSupplies = unprovidedProducts;

                    batch.CreatedBy = username;

                    batch.Notes = SerializeBatchNote(note);
                    batch.Status = "PACKAGING";
                    batch.UpdatedDate = DateTime.UtcNow;
                    break;
                case BatchAction.Delivery:
                    if (batch.Status != "PACKAGING")
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            $"Cannot confirm batch because current status is {batch.Status}. Only PENDING batch can be confirmed.",
                            400
                        );
                    }
                    batch.Status = "DELIVERING";
                    batch.UpdatedDate = DateTime.UtcNow;
                    break;
                case BatchAction.Complete:
                    if (batch.Status != "DELIVERING")
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            $"Cannot complete batch because current status is {batch.Status}. Only PENDING batch can be confirmed.",
                            400
                        );
                    }
                    if (!batch.BatchDetails.Any())
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            "Cannot complete batch because batch has no details.",
                            400
                        );
                    }
                    note = DeserializeBatchNote(batch.Notes);

                    var completedStats = new List<CompletedSupplyStat>();

                    foreach (var detail in batch.BatchDetails)
                    {
                        var required = detail.Quantity;

                        if (!requestItemMap.TryGetValue(detail.BatchDetailId, out var item))
                        {
                            completedStats.Add(new CompletedSupplyStat
                            {
                                BatchDetailId = detail.BatchDetailId,
                                ProductId = detail.ProductId,
                                ProductName = detail.ProductName,
                                Required = required,
                                Provided = 0,
                                Missing = required,
                                Extra = 0,
                                Status = "INSUFFICIENT"
                            });
                            continue;

                        }

                        var provided = item.Quantity;

                        var missing = required > provided ? required - provided : 0;
                        var extra = provided > required ? provided - required : 0;

                        completedStats.Add(new CompletedSupplyStat
                        {
                            BatchDetailId = detail.BatchDetailId,
                            ProductId = detail.ProductId,
                            ProductName = detail.ProductName,
                            Required = required,
                            Provided = provided,
                            Missing = missing,
                            Extra = extra,
                            Status = missing > 0 ? "INSUFFICIENT" : (extra > 0 ? "EXTRA" : "SUFFICIENT")
                        });
                    }
                    var sagaResult = await _batchSagaService.ProcessBatch(request.Items);

                    if (!sagaResult.IsNullOrEmpty())
                    {
                        if(sagaResult.Contains("Product not found or be deleted") || sagaResult.Contains("Batch Detail not found"))
                        {
                            note = DeserializeBatchNote(batch.Notes);
                            note.CancelInfo = new CancelInfoNote
                            {
                                CancelledAt = DateTime.UtcNow,
                                Reason = sagaResult + ". Batch cancelled due to data inconsistency. Please review the batch details and product information, then create a new batch if needed."
                            };

                            batch.Notes = SerializeBatchNote(note);
                            batch.Status = "CANCELED";
                            batch.UpdatedDate = DateTime.UtcNow;
                            
                            await _repo.UpdateBatchAsync(batch);

                                return ApiResponse<BatchDTO>.Error(
                                    null,
                                    sagaResult+ ". Batch cancelled due to data inconsistency.",
                                    409
                                );
                        }
                        return ApiResponse<BatchDTO>.Error(
                             null,
                             sagaResult+ ". Please try again later.",
                             500
                         );
                    }

                    note.CompletedSupplyStats = completedStats;
                    batch.ImageConfirmReceived = SerializeImageItem(request.ImagesJson);
                    batch.Notes = SerializeBatchNote(note);
                    batch.Status = "COMPLETED";
                    batch.UpdatedDate = DateTime.UtcNow;
                    break;
                case BatchAction.Cancel:
                    if (batch.Status == "COMPLETED")
                    {
                        return ApiResponse<BatchDTO>.Error(
                            null,
                            $"Cannot cancel batch because current status is {batch.Status}.",
                            400
                        );
                    }



                    note = DeserializeBatchNote(batch.Notes);
                    
                    note.CancelInfo = new CancelInfoNote
                    {
                        CancelledAt = DateTime.UtcNow,
                        Reason = string.IsNullOrWhiteSpace(request.CancelReason)
                            ? "Batch cancelled"
                            : request.CancelReason
                    };

                    batch.Notes = SerializeBatchNote(note);
                    batch.Status = "CANCELED";
                    batch.UpdatedDate = DateTime.UtcNow;
                    break;

                default:
                    return ApiResponse<BatchDTO>.Error(null, "Invalid action", 400);
            }
            var updated = await _repo.UpdateBatchAsync(batch);

            return ApiResponse<BatchDTO>.Ok(null);
        }
        private BatchNoteModel DeserializeBatchNote(string? noteJson)
        {
            if (string.IsNullOrWhiteSpace(noteJson))
            {
                return new BatchNoteModel();
            }

            try
            {
                return JsonSerializer.Deserialize<BatchNoteModel>(noteJson) ?? new BatchNoteModel();
            }
            catch
            {
                return new BatchNoteModel();
            }
        }

        private string SerializeImageItem(List<ImageItem> images)
        {
            return JsonSerializer.Serialize(images, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private List<ImageItem> DesializeImageItem(string? imagesJson)
        {
            if (string.IsNullOrWhiteSpace(imagesJson))
            {
                return new List<ImageItem>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<ImageItem>>(imagesJson) ?? new  List<ImageItem>();
            }
            catch
            {
                return new List<ImageItem>();
            }
        }

        private string SerializeBatchNote(BatchNoteModel notes)
        {
            return JsonSerializer.Serialize(notes, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        private bool HasPermission(BatchAction action, string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return action switch
            {
                BatchAction.Confirm => role is "3",
                BatchAction.Delivery => role is "3",
                BatchAction.Complete => role is "1",
                BatchAction.Cancel => role is "1",
                _ => false
            };
        }
    }
}
