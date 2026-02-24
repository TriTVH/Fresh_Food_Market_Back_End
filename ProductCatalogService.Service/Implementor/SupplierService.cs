using ProductCatalogService.Model;
using ProductCatalogService.Repository;
using ProductCatalogService.Repository.Implementor;
using ProductCatalogService.Service.DTO;
using ProductCatalogService.Service.DTO.Request;
using ProductCatalogService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.Implementor
{
    public class SupplierService : ISupplierService
    {

        ISupplierRepo _supplierRepo;

        public SupplierService(ISupplierRepo supplierRepo)
        {
            _supplierRepo = supplierRepo;
        }

        public async Task<ApiResponse<SupplierDTO>> CreateSupplierAsync(SupplierModel request)
        {
            var supplier = new Supplier()
            {
                Name = request.Name,
                Address = request.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var createdSupplier = await _supplierRepo.CreateSupplierAsync(supplier);
            
            return ApiResponse<SupplierDTO>.Ok(new SupplierDTO()
            {
                SupplierId = createdSupplier.SupplierId,
                Name = createdSupplier.Name,
                Address = createdSupplier.Address,
                CreatedAt = createdSupplier.CreatedAt,
                UpdatedAt = createdSupplier.UpdatedAt
            }, "Supplier created successfully", 201);
        }
    }
}
