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
    public class BatchDetailRepo : IBatchDetailRepo
    {
        BatchMgmtFfmContext _context;
        public BatchDetailRepo(BatchMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<BatchDetail> GetBatchDetailById(int id)
        {
            return await _context.BatchDetails.FirstOrDefaultAsync(x => x.BatchDetailId == id);
        }
    }
}
