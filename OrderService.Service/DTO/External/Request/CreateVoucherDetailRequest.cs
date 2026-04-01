using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Service.DTO.External.Request
{
    public class CreateVoucherDetailRequest
    {
        public int OrderId { get; set; }
        public List<int> VoucherIds { get; set; } = new();

        public decimal totalAmountOrder { get; set; }
    }
}
