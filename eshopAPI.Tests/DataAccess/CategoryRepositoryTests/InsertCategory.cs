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
    public class InsertCategory
    {
        long _firstCategoryId;
        CategoryRepository _repository;
        DbContextOptions<ShopContext> _options;

        public InsertCategory()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "InsertCategory")
                .Options;
            _repository = GetCategoryRepository();
        }

        [Fact]
        public async void Success()
        {
            Category category = new CategoryBuilder().Build();
            Category newCategory = await _repository.InsertCategory(category);
            await _repository.SaveChanges();
            Category foundCategory = GetCategoryById(category.ID);
            Assert.Equal(category.Name, newCategory.Name);
            Assert.Equal(category.Name, foundCategory.Name);
        }

        [Fact]
        public async void Failure()
        {
            Category category = new CategoryBuilder().Build();
            category.ID = _firstCategoryId;

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                Category newCategory = await _repository.InsertCategory(category);
                await _repository.SaveChanges();
            });
        }
        Category GetCategoryById(long id)
        {
            Category category;
            using (ShopContext context = new ShopContext(_options))
            {
                category = context.Categories.Where(o => o.ID == id).FirstOrDefault();
            }
            return category;
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
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
