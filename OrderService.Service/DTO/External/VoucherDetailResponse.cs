using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.DTO.External
{
    public class VoucherDetailResponse
    {
        public int VoucherDetailId { get; set; }
        public int OrderId { get; set; }
        public int VoucherId { get; set; }
        public decimal DiscountAmount { get; set; }
        public string VoucherCode { get; set; } = string.Empty;
        public DateTime? AppliedDate { get; set; }
    }
}
