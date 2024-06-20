using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}