using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using eshopAPI.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.CategoryRepositoryTests
{
    public class InsertSubcategory
    {
        long _firstCategoryId;
        long _firstSubCategoryId;
        CategoryRepository _repository;
        DbContextOptions<ShopContext> _options;

        public InsertSubcategory()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "InsertSubcategory")
                .Options;
            _repository = GetCategoryRepository();
        }

        [Fact]
        public async void Success()
        {
            SubCategory category = new SubCategoryBuilder().Build();
            SubCategory newCategory = await _repository.InsertSubcategory(category);
            await _repository.SaveChanges();
            Assert.Equal(category.Name, newCategory.Name);
        }

        [Fact]
        public async void Failure()
        {
            SubCategory category = new SubCategory
            {
                ID = _firstSubCategoryId
            };

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                SubCategory newCategory = await _repository.InsertSubcategory(category);
                await _repository.SaveChanges();
            });
        }

        private CategoryRepository GetCategoryRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new CategoryRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            CategoryBuilder categoryBuilder = new CategoryBuilder();
            List<Category> categories = new List<Category>()
            {
                categoryBuilder.New().SetName("ABC").AddSubCategories(2).Build(),
                categoryBuilder.New().SetName("DEF").AddSubCategories(4).Build(),
                categoryBuilder.New().SetName("GHI").Build(),
            };
            _firstCategoryId = categories.First().ID;
            _firstSubCategoryId = categories.First().SubCategories.First().ID;

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
