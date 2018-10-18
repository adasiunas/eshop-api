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

namespace eshopAPI.Tests.DataAccess.ItemRepositoryTests
{
    public class GetAllAdminItemVMAsQueryable
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAllAdminItemVMAsQueryable()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "GetAllAdminItemVMAsQueryable")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            List<AdminItemVM> items = (await _repository.GetAllAdminItemVMAsQueryable()).ToList();
            Assert.Equal(5, items.Count);
        }
        
        private ItemRepository GetItemRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new ItemRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            ItemBuilder builder = new ItemBuilder();
            List<Item> items = new List<Item>()
            {
                builder.New().SetSKU("ABC").AddCategory().AddSubCategory().AddAttributes(3).AddPictures(2).Build(),
                builder.Random().SetSKU("DEF").AddCategory().AddSubCategory().Build(),
                builder.Random().AddCategory().AddSubCategory().Build(),
                builder.Random().AddCategory().AddSubCategory().Build(),
                builder.Random().AddCategory().AddSubCategory().Build()
            };
            _firstItemId = items.First().ID;

            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
