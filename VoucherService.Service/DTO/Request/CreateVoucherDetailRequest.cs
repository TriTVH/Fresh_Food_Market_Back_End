using System.ComponentModel.DataAnnotations;

namespace VoucherService.Service.DTO.Request;

public class CreateVoucherDetailRequest
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int VoucherId { get; set; }
}