using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class ItemVM
    {
        public long ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string MainPicture { get; set; }
        public IEnumerable<ItemPicture> Pictures { get; set; }
        public IEnumerable<ItemAttributesVM> Attributes { get; set; }
        public ItemCategoryVM ItemCategory { get; set; }
    }

    public class ItemCategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public ItemSubcategoryVM Subcategory { get; set; }
    }

    public class ItemSubcategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
    }
}
