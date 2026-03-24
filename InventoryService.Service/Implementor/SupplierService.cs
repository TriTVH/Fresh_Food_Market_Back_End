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
        public async Task<ApiResponse<SupplierDTO>> CreateSupplierAsync(CreateSupplierModel model)
        {
        
            var httpClient = new HttpClient();
            var accountPayload = new
            {
                email = model.AccountEmail,
                password = model.AccountPassword,
                role = model.AccountRole ?? "4"
            };
            var accountContent = new StringContent(JsonSerializer.Serialize(accountPayload), Encoding.UTF8, "application/json");
            HttpResponseMessage accountResp;
            try
            {
                accountResp = await httpClient.PostAsync("http://accservice.api:8080/api/account", accountContent); // Cập nhật URL thực tế
            }
            catch (Exception ex)
            {
                return ApiResponse<SupplierDTO>.Error(default!, $"Lỗi kết nối AccountService: {ex.Message}", 500);
            }
            if (!accountResp.IsSuccessStatusCode)
            {
                return ApiResponse<SupplierDTO>.Error(default!, "Tạo account thất bại", (int)accountResp.StatusCode);
            }
         
            var supplier = new Model.Supplier
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            try
            {
                var created = await _supplierRepo.CreateSupplier(supplier);
                var dto = new SupplierDTO
                {
                    SupplierId = created.SupplierId,
                    Name = created.Name,
                    Address = created.Address,
                    Phone = created.Phone,
                    CreatedAt = created.CreatedAt,
                    UpdatedAt = created.UpdatedAt
                };
                return ApiResponse<SupplierDTO>.Ok(dto, "Tạo Supplier thành công");
            }
            catch (Exception ex)
            {
                await httpClient.DeleteAsync("http://localhost:5001/api/account?email=" + Uri.EscapeDataString(model.AccountEmail));
                return ApiResponse<SupplierDTO>.Error(default!, $"Tạo Supplier thất bại, đã rollback account. Lỗi: {ex.Message}", 500);
            }
        }
    }
}
