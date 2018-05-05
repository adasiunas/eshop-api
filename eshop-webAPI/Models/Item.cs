using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class Item
    {
        [Key]
        public long ID { get; set; } // Primary key
        [Required]
        [MaxLength(10)]
        public string SKU { get; set; } // Bussiness key
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool IsDeleted { get; set; }
        public long SubCategoryID { get; set; }

        public virtual ICollection<AttributeValue> Attributes { get; set; }
        public virtual ICollection<ItemPicture> Pictures { get; set; }
    }

    public static class ItemExtensions
    {
        public static ItemVM GetItemVM(this Item item, SubCategory subCategory)
        {
            return new ItemVM
            {
                ID = item.ID,
                Name = item.Name,
                SKU = item.SKU,
                Description = item.Description,
                Pictures = item.Pictures,
                Attributes = item.Attributes?.Select(i => new ItemAttributesVM { ID = i.ID, AttributeID = i.AttributeID, Name = i.Attribute.Name, Value = i.Value }),
                Price = item.Price,
                MainPicture = item.Pictures?.Select(p => p.URL).FirstOrDefault(),
                ItemCategory = new ItemCategoryVM
                {
                    Name = subCategory.Category.Name,
                    ID = subCategory.Category.ID,
                    Subcategory = new ItemSubcategoryVM
                    {
                        Name = subCategory.Name,
                        ID = subCategory.ID
                    }
                }
            };
        }
    }
}
