using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class UserVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}
