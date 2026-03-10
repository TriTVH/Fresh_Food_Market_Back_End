using InventoryService.Model;
using InventoryService.Service.DTO;
using InventoryService.Service.DTO.Request;
using InventoryService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service
{
    public interface IBatchService
    {
        Task<ApiResponse<BatchDTO>> AddBatchAsync(CreateBatchModel request);
        Task<ApiResponse<BatchDTO>> UpdateBatchAsync(UpdateBatchModel request);
        Task<ApiResponse<BatchDTO>> GetBatchByIdAsync(int id);
        Task<ApiResponse<List<BatchDTO>>> GetAllBatchesAsync();
    }
}
