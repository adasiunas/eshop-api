using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class CategoryBuilder
    {
        private Category _category;

        public long ID { get; set; } = 1;
        public string Name { get; set; } = "CatName";
        public List<SubCategory> SubCategories { get; set; }
        public List<Item> Items { get; set; }

        public CategoryBuilder()
        {
            _category = WithDefaultValues();
        }

        public Category WithDefaultValues()
        {
            var categoriesList = new List<SubCategory>();
            var itemList = new List<Item>();
            _category = new Category
            {
                ID = ID,
                Name = Name,
                Items = itemList,
                SubCategories = categoriesList
            };
            return _category;
        }

        public CategoryBuilder AddSubCategories(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SubCategory subCategory = new SubCategoryBuilder().SetId(i).Build();
                _category.SubCategories.Add(subCategory);
            }
            return this;
        }

        public Category Build()
        {
            return _category;
        }
    }
}
