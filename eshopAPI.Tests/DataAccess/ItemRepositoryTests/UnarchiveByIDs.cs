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
    public class UnarchiveByIDs
    {
        long _firstItemId;
        long _secondItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public UnarchiveByIDs()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "UnarchiveByIDs")
                .Options;
            _repository = GetItemRepository();
        }

        [Fact]
        public async void Success()
        {
            List<long> ids = new List<long>
            {
                _firstItemId,
                _secondItemId
            };

            await _repository.UnarchiveByIDs(ids);
            await _repository.SaveChanges();

            Item itemOne = GetItemById(_firstItemId);
            Item itemTwo = GetItemById(_secondItemId);

            Assert.False(itemOne.IsDeleted);
            Assert.False(itemTwo.IsDeleted);
        }

        Item GetItemById(long id)
        {
            Item item;
            using (ShopContext context = new ShopContext(_options))
            {
                item = context.Items.Where(o => o.ID == id).FirstOrDefault();
            }
            return item;
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
                builder.New().SetSKU("ABC").SetDeleted().AddAttributes(3).AddPictures(2).Build(),
                builder.Random().SetSKU("DEF").SetDeleted().Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            _firstItemId = items.First().ID;
            _secondItemId = items.ElementAt(1).ID;
            context.Items.AddRange(items);

            context.SaveChanges();
        }
    }
}
