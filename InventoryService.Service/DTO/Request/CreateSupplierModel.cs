using System.ComponentModel.DataAnnotations;

namespace InventoryService.Service.DTO.Request
{
    public class CreateSupplierModel
    {
        [Required]
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        // Thông tin account cho AccountService
        [Required]
        public string AccountEmail { get; set; }
        [Required]
        public string AccountPassword { get; set; }
        public string? AccountRole { get; set; }
    }
}

