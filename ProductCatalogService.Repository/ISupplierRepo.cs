using ProductCatalogService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Repository
{
    public interface ISupplierRepo
    {
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
    }
}
