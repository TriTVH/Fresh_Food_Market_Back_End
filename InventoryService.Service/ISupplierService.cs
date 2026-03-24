using InventoryService.Service.DTO;
using InventoryService.Service.DTO.Request;
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
        Task<ApiResponse<SupplierDTO>> CreateSupplierAsync(CreateSupplierModel model);
    }
}
