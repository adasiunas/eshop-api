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
        public int OrderCount { get; set; }
        public decimal MoneySpent { get; set; }
        public decimal AverageMoneySpent { get; set; }
    }
}
