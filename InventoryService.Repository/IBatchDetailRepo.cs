using InventoryService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Repository
{
    public interface IBatchDetailRepo
    {
        Task<BatchDetail> GetBatchDetailById(int id);
    }
}
