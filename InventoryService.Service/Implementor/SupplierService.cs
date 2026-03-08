using InventoryService.Repository;
using InventoryService.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service.Implementors
{
    public class SupplierService : ISupplierService
    {
        ISupplierRepo _supplierRepo;
        public SupplierService(ISupplierRepo supplierRepo)
        {
            _supplierRepo = supplierRepo;
        }
        public async Task<ApiResponse<List<SupplierDTO>>> GetSuppliers()
        {
            var entities = await _supplierRepo.GetSuppliers();
            var dtos = entities.Select(e => new SupplierDTO
            {
                SupplierId = e.SupplierId,
                Name = e.Name,
                Address = e.Address,
                Phone = e.Phone,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
            return ApiResponse<List<SupplierDTO>>.Ok(dtos);
        }
    }
}
