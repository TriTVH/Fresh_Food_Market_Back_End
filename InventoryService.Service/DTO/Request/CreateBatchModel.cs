using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService.Service.DTO.Request
{
    public class CreateBatchModel
    {

        public int SupplierId { get; set; } 

        public List<Item> Items { get; set; }
    }
    public class Item
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateOnly? ExpiredDate { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
