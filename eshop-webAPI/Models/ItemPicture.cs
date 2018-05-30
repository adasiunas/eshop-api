using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class ItemPicture
    {
        [Key]
        public long ID { get; set; }
        [Required]
        [MaxLength(length: 500)]
        public string URL { get; set; }
        public long ItemID { get; set; }
    }
}
