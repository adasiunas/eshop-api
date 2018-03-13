using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class Profile
    {
        [Key]
        public long ID { get; set; } // Primary key
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MaxLength(30)]
        public string Surname { get; set; }
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; }
        [Required]
        public long UserID { get; set; }
        [Required]
        public User User { get; set; } 
    }
}
