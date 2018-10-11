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

namespace eshopAPI.Tests.DataAccess.OrderRepositoryTests
{
    public class FindByOrderNumber
    {
        long _firstOrderId;
        OrderRepository _repository;
        DbContextOptions<ShopContext> _options;

        public FindByOrderNumber()
        {
            _options = new DbContextOptionsBuilder<ShopContext>()
                .UseInMemoryDatabase(databaseName: "FindByOrderNumber")
                .Options;
            _repository = GetOrderRepository();
        }

        [Fact]
        public async void Success()
        {
            Order expectedOrder = GetOrderById(_firstOrderId);
            Order order = await _repository.FindByOrderNumber(expectedOrder.OrderNumber);

            Assert.Equal(expectedOrder.DeliveryAddress, order.DeliveryAddress);
            Assert.Equal(expectedOrder.Items.Count, order.Items.Count);
        }

        [Fact]
        public async void Failure()
        {
            Order order = await _repository.FindByOrderNumber(_firstOrderId - 1);

            Assert.Null(order);
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

        private OrderRepository GetOrderRepository()
        {
            ShopContext dbContext = new ShopContext(_options);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            using (ShopContext context = new ShopContext(_options))
            {
                SeedData(context);
            }
            return new OrderRepository(dbContext);
        }

        void SeedData(ShopContext context)
        {
            OrderBuilder builder = new OrderBuilder();
            List<Order> orders = new List<Order>()
            {
                builder.New().AddItems(2).Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build(),
                builder.Random().Build()
            };
            _firstOrderId = orders.First().ID;

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
