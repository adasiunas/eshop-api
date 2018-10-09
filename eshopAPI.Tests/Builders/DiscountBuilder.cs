using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class DiscountBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public string Name { get; set; } = "DiscountName";
        public int Value { get; set; } = 10;
        public bool IsPercentages { get; set; } = true;
        public DateTime To { get; set; } = DateTime.UtcNow.AddDays(1);

        static int _id = 1;
        Discount _discount;

        public DiscountBuilder()
        {
            _discount = WithDefaultValues();
        }

        public DiscountBuilder New()
        {
            _discount = WithDefaultValues();
            return this;
        }

        public DiscountBuilder SetName(string name)
        {
            _discount.Name = name;
            return this;
        }

        public DiscountBuilder SetItem(Item item)
        {
            ClearItem();
            _discount.Item = item;
            _discount.ItemID = item.ID;
            return this;
        }

        public DiscountBuilder SetCategory(Category item)
        {
            ClearItem();
            _discount.Category = item;
            _discount.CategoryID = item.ID;
            return this;
        }

        public DiscountBuilder SetSubcategory(SubCategory item)
        {
            ClearItem();
            _discount.SubCategory = item;
            _discount.SubCategoryID = item.ID;
            return this;
        }

        public DiscountBuilder SetDate(DateTime to)
        {
            _discount.To = to;
            return this;
        }

        public Discount Build()
        {
            _discount.ID = ID;
            return _discount;
        }

        void ClearItem()
        {
            _discount.Item = null;
            _discount.ItemID = null;
            _discount.Category = null;
            _discount.CategoryID = null;
            _discount.SubCategory = null;
            _discount.SubCategoryID = null;
        }

        Discount WithDefaultValues()
        {
            _discount = new Discount
            {
                To = To,
                Name = Name,
                IsPercentages = IsPercentages,
                Value = Value
            };
            return _discount;
        }
    }
}
