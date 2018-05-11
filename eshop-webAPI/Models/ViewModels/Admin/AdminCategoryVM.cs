using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels.Admin
{
    public class AdminCategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public bool HasDescendants { get; set; }
    }
}
