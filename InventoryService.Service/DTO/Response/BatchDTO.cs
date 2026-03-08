using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service.DTO.Response
{
    public class BatchDTO
    {
        public int BatchId { get; set; }

        public string SupplierName { get; set; }

        public string SupplierPhone { get; set; }

        public string SupplierAddress { get; set; }

        public string CreatedBy { get; set; } = null!;

        public string BatchCode { get; set; } = null!;

        public int? TotalItems { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? Status { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public string? ImageConfirmReceived { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<BatchDeatailDTO> BatchDetails { get; set; }
    }
    public class BatchDeatailDTO
    {
        public int BatchDetailId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }

        public DateOnly? ExpiredDate { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
