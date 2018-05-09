using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class SubCategory
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } // Bussiness key
        [ForeignKey("SubCategory")]
        public long CategoryID { get; set; }
        public Category Category { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
