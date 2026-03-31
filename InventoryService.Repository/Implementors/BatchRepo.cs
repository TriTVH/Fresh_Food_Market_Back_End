using InventoryService.Model;
using InventoryService.Model.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Repository.Implementors
{
    public class BatchRepo : IBatchRepo
    {
        BatchMgmtFfmContext _context;
        public BatchRepo(BatchMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<Batch> AddBatchAsync(Batch batch)
        {
             var entry = await _context.Batches.AddAsync(batch);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
        public async Task<Batch> UpdateBatchAsync(Batch batch)
        {
            var entry = _context.Batches.Update(batch);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
        public async Task<Batch> DeleteBatchAsync(Batch batch)
        {
            var entry = _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();
            return entry.Entity;
        }
        public async Task<Batch?> GetBatchByIdAsync(int id)
        {
            return await _context.Batches
                .Include(x => x.BatchDetails)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.BatchId == id);
        }
        public async Task<List<Batch>> GetAllBatchesAsync()
        {
            return await _context.Batches.Include(x => x.Supplier)
                .ToListAsync();
        }
        public async Task<int> CountBatchBySupplierAsync(int supplierId)
        {
            return await _context.Batches
                .Where(x => x.SupplierId == supplierId)
                .CountAsync();
        }
    }
}
