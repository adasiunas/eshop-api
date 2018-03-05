using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class OrderItem
    {
        [Key]
        public long ID { get; set; }
        public long ItemID { get; set; }
        public Item Item { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
