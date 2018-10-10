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
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
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

        static int _id = 1;

        Item _item;

        public ItemBuilder()
        {
            _item = WithDefaultValues();
        }

        public ItemBuilder New()
        {
            _item = WithDefaultValues();
            return this;
        }

        Item WithDefaultValues()
        {
            var attributeValues = new List<AttributeValue>();
            var pictures = new List<ItemPicture>();
            _item = new Item
            {
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
                Name = rnd.RandomString(10),
                SKU = rnd.RandomString(3),
                Description = rnd.RandomString(20),
                Price = rnd.Next(),
                Attributes = new List<AttributeValue>(),
                Pictures = new List<ItemPicture>()
            };
            AddCategory();
            AddSubCategory();

            int attributesCnt = rnd.Next(1, 5);
            AddAttributes(attributesCnt);

            int picturesCnt = rnd.Next(1, 5);
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
                ItemPicture picture = new ItemPictureBuilder().Build();
                _item.Pictures.Add(picture);
            }
            return this;
        }

        public ItemBuilder AddCategory()
        {
            _item.Category = new CategoryBuilder().Build();
            _item.CategoryID = _item.Category.ID;
            return this;
        }

        public ItemBuilder AddSubCategory()
        {
            _item.SubCategory = new SubCategoryBuilder().Build();
            _item.SubCategoryID = _item.SubCategory.ID;
            _item.Category.SubCategories.Add(_item.SubCategory);
            return this;
        }

        public ItemBuilder SetSKU(string sku)
        {
            _item.SKU = sku;
            return this;
        }

        public ItemBuilder SetDeleted(bool deleted = true)
        {
            _item.IsDeleted = deleted;
            _item.DeleteDate = DateTime.UtcNow;
            return this;
        }

        public Item Build()
        {
            _item.ID = ID;
            return _item;
        }
    }
}
