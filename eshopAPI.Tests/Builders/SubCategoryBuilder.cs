using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class SubCategoryBuilder
    {
        public static int LastId { get { return _id - 1; } }

        public long ID { get { return _id++; } }
        public long CategoryId { get; set; } = 1;
        public string Name { get; set; } = "SubCatName";

        private SubCategory _subCategory;
        static int _id = 1;

        public SubCategoryBuilder()
        {
            _subCategory = WithDefaultValues();
        }
        
        public SubCategoryBuilder New()
        {
            _subCategory = WithDefaultValues();
            return this;
        }

        public SubCategoryBuilder SetCategoryId(long id)
        {
            _subCategory.CategoryID = id;
            return this;
        }

        public SubCategory WithDefaultValues()
        {
            _subCategory = new SubCategory
            {
                Name = Name,
                CategoryID = CategoryId
            };
            return _subCategory;
        }

        public SubCategory Build()
        {
            _subCategory.ID = ID;
            return _subCategory;
        }
    }
}
