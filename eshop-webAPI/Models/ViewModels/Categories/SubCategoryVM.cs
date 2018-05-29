using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels
{
    public class SubCategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
    }

    public static class SubCategoryExtensions
    {
        public static SubCategoryVM GetSubCategoryVM(this SubCategory subCategory)
        {
            return new SubCategoryVM
            {
                Name = subCategory.Name,
                ID = subCategory.ID
            };
        }
    }
}
