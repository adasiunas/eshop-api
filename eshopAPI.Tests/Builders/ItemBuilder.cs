using eshopAPI.Models;
using eshopAPI.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class ItemBuilder
    {
        Item _item;
        int _id = 1;
        public long ID { get
            {
                return _id++;
            }
        }
        public string SKU { get; set; } = "ABC1";
        public string Name { get; set; } = "ItemA";
        public string Description { get; set; } = "Description of this item";
        public decimal Price { get; set; } = 10;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
        public DateTime? DeleteDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        public long CategoryID { get; set; } = 2;

        public long? SubCategoryID { get; set; } = 3;

        public List<AttributeValue> Attributes { get; set; }
        public List<ItemPicture> Pictures { get; set; }

        public ItemBuilder()
        {
            _item = WithDefaultValues();
        }
        public Item WithDefaultValues()
        {
            var attributeValues = new List<AttributeValue>();
            var pictures = new List<ItemPicture>();
            _item = new Item
            {
                ID = ID,
                Name = Name,
                SKU = SKU,
                Description = Description,
                Price = Price,
                Attributes = attributeValues,
                CategoryID = CategoryID,
                SubCategoryID = SubCategoryID,
                Pictures = pictures
            };
            return _item;
        }

        public ItemBuilder Random()
        {
            Random rnd = new Random();
            _item = new Item
            {
                ID = rnd.Next(),
                Name = rnd.RandomString(10),
                SKU = rnd.RandomString(3),
                Description = rnd.RandomString(20),
                Price = rnd.Next(),
                Attributes = new List<AttributeValue>(),
                CategoryID = rnd.Next(),
                SubCategoryID = rnd.Next(),
                Pictures = new List<ItemPicture>()
            };
            int attributesCnt = rnd.Next(5);
            AddAttributes(attributesCnt);

            int picturesCnt = rnd.Next(5);
            AddPictures(picturesCnt);

            return this;
        }

        public ItemBuilder AddAttributes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AttributeValue attributeValue = new AttributeValueBuilder().WithDefaultValues();
                _item.Attributes.Add(attributeValue);
            }
            return this;
        }
        public ItemBuilder AddPictures(int count)
        {
            Random rng = new Random();
            for (int i = 0; i < count; i++)
            {
                ItemPicture picture = new ItemPicture
                {
                    ID = i,
                    ItemID = _item.ID,
                    URL = rng.RandomString(30)
                };
                _item.Pictures.Add(picture);
            }
            return this;
        }

        public Item Build()
        {
            return _item;
        }
    }
}
