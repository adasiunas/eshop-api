using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels.Admin
{
    public class AdminDiscountVM
    {
        public long ID { get; set; } // Primary key
        public string Name { get; set; }
        public int Value { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
    }
}
