using eshopAPI.Models.ViewModels;
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
        public IEnumerable<ItemPictureVM> Pictures { get; set; }
        public IEnumerable<ItemAttributesVM> Attributes { get; set; }
        public ItemCategoryVM Category { get; set; }
        public ItemSubCategoryVM SubCategory { get; set; }
        public byte[] OptLockVersion { get; set; }
    }

    public class ItemCategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
    }

    public class ItemSubCategoryVM
    {
        public string Name { get; set; }
        public long ID { get; set; }
    }

    public static class ItemExtensions
    {
        public static Item UpdateItem(this Item initialItem, Item itemToUpdateTo)
        {
            initialItem.Attributes = itemToUpdateTo.Attributes;
            initialItem.Description = itemToUpdateTo.Description;
            initialItem.ModifiedDate = DateTime.Now;
            initialItem.Name = itemToUpdateTo.Name;
            initialItem.Pictures = itemToUpdateTo.Pictures;
            initialItem.Price = itemToUpdateTo.Price;
            initialItem.SKU = itemToUpdateTo.SKU;
            initialItem.SubCategoryID = itemToUpdateTo.SubCategoryID;
            return initialItem;
        }
        public static ItemVM GetItemVM(this Item item)
        {
            return new ItemVM
            {
                ID = item.ID,
                Name = item.Name,
                SKU = item.SKU,
                Description = item.Description,
                Pictures = item.Pictures?.Select(p => new ItemPictureVM { ID = p.ID, URL = p.URL }),
                Attributes = item.Attributes?.Select(i => new ItemAttributesVM { ID = i.ID, AttributeID = i.AttributeID, Name = i.Attribute.Name, Value = i.Value }),
                Price = item.Price,
                Category = new ItemCategoryVM
                {
                    Name = item.SubCategory.Category.Name,
                    ID = item.SubCategory.Category.ID
                },
                SubCategory = item.SubCategory == null ? null : new ItemSubCategoryVM
                {
                    Name = item.SubCategory.Name,
                    ID = item.SubCategory.ID
                }
            };
        }
    }
}
