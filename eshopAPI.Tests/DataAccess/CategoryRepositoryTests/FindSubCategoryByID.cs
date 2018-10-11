﻿using eshopAPI.DataAccess;
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
    public class FindSubCategoryByID
    {
        long _firstCategoryId;
        CategoryRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindSubCategoryByID()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindSubCategoryByID")
                .Options;
            _repository = GetCategoryRepository();
        }

        [Fact]
        public async void Success()
        {
            SubCategory subCategory = GetSubCategory();
            SubCategory foundCategory = await _repository.FindSubCategoryByID(subCategory.ID);
            Assert.Equal(subCategory.ID, foundCategory.ID);
            Assert.Equal(subCategory.CategoryID, foundCategory.CategoryID);
        }

        [Fact]
        public async void Failure()
        {
            SubCategory foundCategory = await _repository.FindSubCategoryByID(-1);
            Assert.Null(foundCategory);
        }

        SubCategory GetSubCategory()
        {
            SubCategory category;
            using (ShopContext context = new ShopContext(_options))
            {
                category = context.SubCategories.Include(o => o.Category).First();
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