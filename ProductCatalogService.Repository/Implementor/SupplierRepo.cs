using ProductCatalogService.Model;
using ProductCatalogService.Model.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Repository.Implementor
{
    public class SupplierRepo : ISupplierRepo
    {
        ProductCatalogMgmtFfmContext _context;
        public SupplierRepo(ProductCatalogMgmtFfmContext context)
        {
            _context = context;
        }
        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }
    }
}
