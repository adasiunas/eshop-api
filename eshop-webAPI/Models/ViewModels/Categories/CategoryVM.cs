using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels
{
    public class CategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public ICollection<SubCategoryVM> SubCategories { get; set; }
    }

    public static class CategoryExtensions
    {
        public static CategoryVM GetCategoryVM(this Category category)
        {
            return new CategoryVM
            {
                Name = category.Name,
                ID = category.ID,
                SubCategories = category.SubCategories.OrderBy(sc => sc.Name).Select(sc => sc.GetSubCategoryVM()).ToList()
            };
        }
    }
}
