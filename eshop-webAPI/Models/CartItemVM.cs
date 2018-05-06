using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models
{
    public class CartItemVM
    {
        public long ID { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string MainPicture { get; set; }
        public decimal Price { get; set; }
        public virtual ICollection<ItemAttributesVM> Attributes { get; set; }
        public int Count { get; set; }
    }

    public static class CartItemVMExtensions
    {
        public static CartItemVM GetCartItemVM(this CartItem item)
        {
            return new CartItemVM
            {
                ID = item.ID,
                SKU = item.Item.SKU,
                Name = item.Item.Name,
                MainPicture = item.Item.Pictures.FirstOrDefault()?.URL,
                Price = item.Item.Price,
                Count = item.Count,
                Attributes = item.Item.Attributes
                    .Select(i => new ItemAttributesVM
                    {
                        ID = i.ID, AttributeID = i.AttributeID, Name = i.Attribute.Name, Value = i.Value
                    })
                    .Take(2).ToList()
            };
        }
    }
}
