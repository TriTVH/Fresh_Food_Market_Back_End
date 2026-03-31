using InventoryService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Repository
{
    public interface IBatchRepo
    {
        Task<Batch> AddBatchAsync(Batch batch);
        Task<Batch> UpdateBatchAsync(Batch batch);
        Task<Batch> DeleteBatchAsync(Batch batch);
        Task<Batch?> GetBatchByIdAsync(int id);
        Task<List<Batch>> GetAllBatchesAsync();
        Task<int> CountBatchBySupplierAsync(int supplierId);
    }
}
