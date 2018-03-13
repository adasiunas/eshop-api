using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class AttributeValue
    {
        [Key]
        public long ID { get; set; }
        [Required]
        public long AttributeID { get; set; }
        [Required]
        public Attribute Attribute { get; set; }
        [Required]
        [MaxLength(100)]
        public string Value { get; set; }
    }
}
