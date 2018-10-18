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

namespace eshopAPI.Tests.DataAccess.DiscountRepositoryTests
{
    public class GetDiscounts
    {
        long _firstDiscountId;
        DiscountRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetDiscounts()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "GetDiscounts")
                .Options;
            _repository = GetDiscountRepository();
        }

        [Fact]
        public async void Success()
        {
            List<Discount> discounts = await _repository.GetDiscounts();
            Assert.Equal(11, discounts.Count);
        }

        private DiscountRepository GetDiscountRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new DiscountRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            DiscountBuilder builder = new DiscountBuilder();
            List<Discount> discounts = new List<Discount>()
            {
                builder.New().SetItem(new Item { ID = 1 }).Build(),
                builder.New().SetItem(new Item { ID = 1 }).Build(),
                builder.New().SetItem(new Item { ID = 3 }).Build(),
                builder.New().SetItem(new Item { ID = 3 }).SetDate(DateTime.UtcNow.AddDays(-1)).Build(),
                builder.New().SetCategory(new Category { ID = 1 }).Build(),
                builder.New().SetCategory(new Category { ID = 3 }).Build(),
                builder.New().SetCategory(new Category { ID = 3 }).Build(),
                builder.New().SetCategory(new Category { ID = 4 }).Build(),
                builder.New().SetSubcategory(new SubCategory { ID = 1 }).Build(),
                builder.New().SetSubcategory(new SubCategory { ID = 2 }).Build(),
                builder.New().SetSubcategory(new SubCategory { ID = 1 }).Build(),
                builder.New().SetSubcategory(new SubCategory { ID = 1 }).Build(),
            };
            _firstDiscountId = discounts.First().ID;

            context.Discounts.AddRange(discounts);
            context.SaveChanges();
        }
    }
}
