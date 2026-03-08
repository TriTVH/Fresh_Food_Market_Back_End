using InventoryService.Model;
using InventoryService.Model.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Repository.Implementors
{
    public class SupplierRepo : ISupplierRepo
    {
        BatchMgmtFfmContext _context;
        public SupplierRepo(BatchMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<List<Supplier>> GetSuppliers()
        {
            return _context.Suppliers.ToList();
        }
    }
}
