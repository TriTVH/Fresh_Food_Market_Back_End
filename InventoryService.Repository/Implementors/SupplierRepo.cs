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
        public async Task<Supplier> GetSupplierById(int supplierId)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == supplierId);
        }
        public async Task<Supplier> CreateSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }
    }
}
