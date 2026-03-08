using InventoryService.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service
{
    public interface ISupplierService
    {
        Task<ApiResponse<List<SupplierDTO>>> GetSuppliers();
    }
}
