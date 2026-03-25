using InventoryService.Repository;
using InventoryService.Service.DTO;
using InventoryService.Service.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        //public async Task<ApiResponse<SupplierDTO>> CreateSupplierAsync(CreateSupplierModel model)
        //{
                
        //}
    }
}
