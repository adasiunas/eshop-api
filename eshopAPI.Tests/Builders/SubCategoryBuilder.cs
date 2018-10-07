using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace eshopAPI.Tests.Builders
{
    public class SubCategoryBuilder
    {
        private SubCategory _subCategory;

        public long ID { get; set; } = 1;
        public string Name { get; set; } = "SubCatName";

        public SubCategoryBuilder()
        {
            _subCategory = WithDefaultValues();
        }
        
        public SubCategoryBuilder SetId(int id)
        {
            _subCategory.ID = id;
            return this;
        }

        public SubCategory WithDefaultValues()
        {
            _subCategory = new SubCategory
            {
                ID = ID,
                Name = Name
            };
            return _subCategory;
        }

        public SubCategory Build()
        {
            return _subCategory;
        }
    }
}
