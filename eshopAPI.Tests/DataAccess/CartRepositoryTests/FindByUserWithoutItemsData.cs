using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Tests.Builders;
using eshopAPI.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.DataAccess.CartRepositoryTests
{
    public class FindByUserWithoutItemsData
    {
        string _email = "test@test.com";
        CartRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindByUserWithoutItemsData()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindByUserWithoutItemsData")
                .Options;
            _repository = GetCartRepository();

        }

        [Fact]
        public async void FindSuccess()
        {
            // According to method name no data about item should be returned.
            // However it is. Dunno if it is bad naming or wrong implementation
            Cart cart = await _repository.FindByUserWithoutItemsData(_email);
            Assert.Equal(3, cart.Items.Count);
        }

        [Fact]
        public async void FindFailure()
        {
            Cart cart = await _repository.FindByUserWithoutItemsData("random@email.com");
            Assert.Null(cart);
        }
        
        private CartRepository GetCartRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new CartRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            ItemBuilder itemBuilder = new ItemBuilder();
            ShopUser user = new ShopUser
            {
                Id = "1",
                Name = "TestUser",
                Email = _email,
                NormalizedEmail = _email.Normalize()
            };
            context.Users.Add(user);
            CartItemBuilder cartItemBuilder = new CartItemBuilder();
            List<CartItem> items = new List<CartItem>
            {
                cartItemBuilder.Random().Build(),
                cartItemBuilder.Random().Build(),
                cartItemBuilder.Random().Build()
            };
            context.CartItems.AddRange(items);
            context.Carts.AddRange(new List<Cart>
            {
                new Cart { ID = 1, User = user, Items = items },
            });
            context.SaveChanges();
        }
    }
}
