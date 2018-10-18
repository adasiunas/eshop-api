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
    public class ArchiveByIDs
    {
        long _firstItemId;
        long _secondItemId;
        ItemRepository _repository;
        DbContextOptions<ShopContext> _options;

        public ArchiveByIDs()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "ArchiveByIDs")
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

            await _repository.ArchiveByIDs(ids);
            await _repository.SaveChanges();

            Item itemOne = GetItemById(_firstItemId);
            Item itemTwo = GetItemById(_secondItemId);
            CartItem cartItemOne = GetCartItemById(_firstItemId);
            CartItem cartItemTwo = GetCartItemById(_secondItemId);

            Assert.True(itemOne.IsDeleted);
            Assert.True(itemTwo.IsDeleted);
            Assert.Null(cartItemOne);
            Assert.Null(cartItemTwo);
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

        CartItem GetCartItemById(long id)
        {
            CartItem item;
            using (ShopContext context = new ShopContext(_options))
            {
                item = context.CartItems.Where(o => o.ItemID == id).FirstOrDefault();
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
                builder.New().SetSKU("ABC").AddAttributes(3).AddPictures(2).Build(),
                builder.Random().New().SetSKU("DEF").Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            _firstItemId = items.First().ID;
            _secondItemId = items.ElementAt(1).ID;
            context.Items.AddRange(items);

            List<CartItem> cartItems = new List<CartItem>{
                new CartItemBuilder().SetItem(items.ElementAt(0)).Build(),
                new CartItemBuilder().SetItem(items.ElementAt(1)).Build(),
            };
            context.CartItems.AddRange(cartItems);

            context.SaveChanges();
        }
    }
}
