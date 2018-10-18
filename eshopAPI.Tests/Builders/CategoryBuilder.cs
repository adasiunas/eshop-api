using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class CategoryBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public string Name { get; set; } = "CatName";
        public List<SubCategory> SubCategories { get; set; }
        public List<Item> Items { get; set; }

        static int _id = 1;
        Category _category;

        public CategoryBuilder()
        {
            _category = WithDefaultValues();
        }

        public CategoryBuilder New()
        {
            _category = WithDefaultValues();
            return this;
        }

        Category WithDefaultValues()
        {
            var categoriesList = new List<SubCategory>();
            var itemList = new List<Item>();
            _category = new Category
            {
                Name = Name,
                Items = itemList,
                SubCategories = categoriesList
            };
            return _category;
        }

        public CategoryBuilder SetName(string name)
        {
            _category.Name = name;
            return this;
        }

        public CategoryBuilder AddSubCategories(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SubCategory subCategory = new SubCategoryBuilder().SetCategoryId(_category.ID).Build();
                _category.SubCategories.Add(subCategory);
            }
            return this;
        }

        public Category Build()
        {
            _category.ID = ID;
            return _category;
        }
    }
}
