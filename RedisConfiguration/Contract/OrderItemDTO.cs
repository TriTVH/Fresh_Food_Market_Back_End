using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConfiguration.DTOs
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPerItem { get; set; }
        public decimal SubTotal { get; set; }
        public int Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
