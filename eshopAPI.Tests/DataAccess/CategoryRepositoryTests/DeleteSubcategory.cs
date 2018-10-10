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
    public class DeleteSubcategory
    {
        long _firstCategoryId;
        long _firstSubCategoryId;
        CategoryRepository _repository;
        DbContextOptions<ShopContext> _options;

        public DeleteSubcategory()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "DeleteSubcategory")
                .Options;
            _repository = GetCategoryRepository();
        }

        [Fact]
        public async void Success()
        {
            SubCategory category = new SubCategory { ID = _firstSubCategoryId };
            SubCategory deletedCategory = await _repository.DeleteSubcategory(category);
            await _repository.SaveChanges();
            SubCategory foundCategory = GetSubcategoryById(_firstCategoryId);
            Assert.Null(foundCategory);
        }

        [Fact]
        public async void Failure()
        {
            SubCategory category = new SubCategory { ID = _firstSubCategoryId - 1 };

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                SubCategory deletedCategory = await _repository.DeleteSubcategory(category);
                await _repository.SaveChanges();
            });
        }

        SubCategory GetSubcategoryById(long id)
        {
            SubCategory category;
            using (ShopContext context = new ShopContext(_options))
            {
                category = context.SubCategories.Where(o => o.ID == id).FirstOrDefault();
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
            _firstSubCategoryId = categories.First().SubCategories.First().ID;
            context.Categories.AddRange(categories);
            context.SaveChanges();
        }
    }
}
