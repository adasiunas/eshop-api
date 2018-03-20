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
        public long ID { get; set; } // Primary key
        [Required]
        public long ItemID { get; set; }
        [Required]
        public Item Item { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
