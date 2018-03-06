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
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public long UserID { get; set; }
        [Required]
        public User User { get; set; } 
    }
}
