using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductCatalogService.Service.DTO.Request
{
    public class UpdateProductQtyRequest
    {
        public int ProductId { get; set; }
        public int ProductQty { get; set; }
        public UpdateQtyAction Action { get; set; }
    }

    public enum UpdateQtyAction
    {
        Add,
        Subtract
    }

}
