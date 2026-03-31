using System.ComponentModel.DataAnnotations;

namespace VoucherService.Service.DTO.Request;

public class CreateVoucherDetailRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Order Id must be greater than 0")]
    public int OrderId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Voucher Ids must contain at least 1 item")]
    public List<int> VoucherIds { get; set; } = new();


    [Range(1, int.MaxValue, ErrorMessage = "Total amount order must be greater than 0")]
    public decimal totalAmountOrder { get; set; }
}