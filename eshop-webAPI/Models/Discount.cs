using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class Discount
    {
        [Key]
        public long ID { get; set; } // Primary key

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int Value { get; set; }

        public DateTime To { get; set; }

        public bool IsPercentages { get; set; }

        public Item Item { get; set; }

        public Category Category { get; set; }

        public SubCategory SubCategory { get; set; }
    }
}
