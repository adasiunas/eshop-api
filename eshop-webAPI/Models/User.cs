using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class User
    {
        [Key]
        public long ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public bool Approved { get; set; }
    }

    public enum UserRole
    {
        User,
        Admin,
        Blocked
    }
}
