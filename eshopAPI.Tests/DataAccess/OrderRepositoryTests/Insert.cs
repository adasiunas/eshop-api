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
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace eshopAPI.Tests.DataAccess.OrderRepositoryTests
{
    public class Insert
    {
        long _firstOrderId;
        OrderRepository _repository;
        DbContextOptions<ShopContext> _options;
        private readonly ITestOutputHelper output;

        public Insert(ITestOutputHelper _output)
        {
            output = _output;
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "Insert")
                .Options;
        }

        [Fact]
        public async void Success()
        {
            _repository = await GetOrderRepository(_options);
            // Find existing user from same context, otherwise ef tries to add new record and fails
            ShopUserRepository userRepository = new ShopUserRepository(_repository.Context);
            ShopUser user = await userRepository.GetUserWithEmail("test@test.com");

            Order newOrder = new OrderBuilder().SetUser(user).AddItems(1).Build();
            output.WriteLine($"New orderId: {newOrder.ID}");
            Order order = await _repository.Insert(newOrder);
            await _repository.SaveChanges();

            Order foundOrder = GetOrderById(newOrder.ID);

            Assert.Equal(newOrder.OrderNumber, order.OrderNumber);
            Assert.Equal(newOrder.OrderNumber, foundOrder.OrderNumber);
            Assert.Equal(newOrder.Items.Count, foundOrder.Items.Count);
        }

        [Fact]
        public async void Failure()
        {
            _repository = await GetOrderRepository(_options);
            Order newOrder = new OrderBuilder().Random().SetItems(null).Build();
            newOrder.ID = _firstOrderId;

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                Order order = await _repository.Insert(newOrder);
                await _repository.SaveChanges();
            });
        }

        Order GetOrderById(long id)
        {
            Order order;
            using (ShopContext context = new ShopContext(_options))
            {
                order = context.Orders
                    .Where(o => o.ID == id)
                    .Include(o => o.User)
                    .Include(o => o.Items)
                    .FirstOrDefault();
            }
            return order;
        }

        async Task<OrderRepository> GetOrderRepository(DbContextOptions<ShopContext> options)
        {
            ShopContext dbContext = new ShopContext(options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(options))
            {
                await SeedData(context);
            }
            return new OrderRepository(dbContext);
        }

        async Task SeedData(ShopContext context)
        {
            OrderBuilder builder = new OrderBuilder();
            List<Order> orders = new List<Order>()
            {
                builder.New().AddItems(2).Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            _firstOrderId = orders.First().ID;

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();
        }
    }
}
