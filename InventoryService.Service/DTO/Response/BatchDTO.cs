using InventoryService.Service.DTO.Request;
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

        public string? SupplyBy { get; set; } = null!;

        public string BatchCode { get; set; } = null!;

        public int? TotalItems { get; set; }

        public decimal? TotalPrice { get; set; }

        public string? Status { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public List<ImageItem>? ImageConfirmReceived { get; set; }

        public BatchNoteModel? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<BatchDetailDTO> BatchDetails { get; set; }
    }
    public class BatchDetailDTO
    {
        public int BatchDetailId { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateOnly? ExpiredDate { get; set; }
    }
}
