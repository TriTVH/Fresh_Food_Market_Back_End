using InventoryService.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryService.Service.DTO.Request
{
    public class UpdateBatchModel
    {
        public int Id { get; set; }
        public List<UpdateItem> Items { get; set; }
        public BatchAction Action { get; set; }
        [MinLength(1, ErrorMessage = "At least one image is required")]
        public List<ImageItem> ImagesJson { get; set; } = new List<ImageItem>();
        public string CancelReason { get; set; }
    }
    public enum BatchAction
    {
        Confirm,
        Delivery,
        Complete,
        Cancel
    }
    public class UpdateItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateOnly? ExpiredDate { get; set; }

    }

    public class MissingSupplyNote
    {
        [JsonPropertyName("batchDetailId")]
        public int BatchDetailId { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("required")]
        public int Required { get; set; }

        [JsonPropertyName("provided")]
        public int Provided { get; set; }

        [JsonPropertyName("missing")]
        public int Missing { get; set; }
    }

    public class CancelInfoNote
    {
        [JsonPropertyName("cancelledAt")]
        public DateTime CancelledAt { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }

    public class CompletedSupplyStat
    {
        [JsonPropertyName("batchDetailId")]
        public int BatchDetailId { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("required")]
        public int Required { get; set; }

        [JsonPropertyName("provided")]
        public int Provided { get; set; }

        [JsonPropertyName("missing")]
        public int Missing { get; set; }

        [JsonPropertyName("extra")]
        public int Extra { get; set; }


        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class BatchNoteModel
    {
        [JsonPropertyName("insufficientSupplyNote")]
        public List<MissingSupplyNote> InsufficientSupplyNote { get; set; } = new();

        [JsonPropertyName("unprovidedProducts")]
        public List<MissingSupplyNote> UndeliverableSupplies { get; set; } = new();

        [JsonPropertyName("completedSupplyStats")]
        public List<CompletedSupplyStat> CompletedSupplyStats { get; set; } = new();

        [JsonPropertyName("cancelInfo")]
        public CancelInfoNote? CancelInfo { get; set; }
    }

}
