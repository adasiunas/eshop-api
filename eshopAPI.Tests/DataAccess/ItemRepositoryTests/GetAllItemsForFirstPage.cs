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
    public class GetAllItemsForFirstPage
    {
        long _firstItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAllItemsForFirstPage()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "GetAllItemsForFirstPage")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            List<ItemVM> items = await _repository.GetAllItemsForFirstPage();
            ItemVM first = items.First();
            Assert.Equal(4, items.Count);
            Assert.Equal(3, first.Attributes.ToList().Count);
            Assert.Equal(2, first.Pictures.ToList().Count);
            Assert.NotNull(first.Category);
            Assert.NotNull(first.SubCategory);
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
                builder.Random().AddCategory().Build(),
                builder.Random().AddCategory().AddSubCategory().SetDeleted().Build()
            };
            _firstItemId = items.First().ID;

            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
