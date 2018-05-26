using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels
{
    public class OrderItemVM
    {
        public long ID { get; set; }
        public string SKU { get; set; }
        public long ItemID { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

    }
}
