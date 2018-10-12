using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Models.ViewModels.Admin;
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
    public class GetAllOrdersAsQueryable
    {
        long _firstOrderId;
        string _userEmail;
        OrderRepository _repository;
        DbContextOptions<ShopContext> _options;

        public GetAllOrdersAsQueryable()
        {
            _userEmail = "123@abc.xyz";
            _options = new DbContextOptionsBuilder<ShopContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: "GetAll_OrdersAsQueryable")
                .Options;
            _repository = GetOrderRepository();
        }

        [Fact]
        public async void UserOrders()
        {
            List<OrderVM> userOneOrders = (await _repository.GetAllOrdersAsQueryable(_userEmail)).ToList();
            Assert.Single(userOneOrders);

            Order order = GetOrderById(_firstOrderId);
            List<OrderVM> userTwoOrders = (await _repository.GetAllOrdersAsQueryable(order.User.Email)).ToList();
            Assert.Equal(4, userTwoOrders.Count);
        }

        [Fact]
        public async void AdminOrders()
        {
            List<AdminOrderVM> orders = (await _repository.GetAllAdminOrdersAsQueryable()).ToList();

            Assert.Equal(5, orders.Count);
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
            ShopUser user = new ShopUserBuilder().SetEmail(_userEmail).Build();
            OrderBuilder builder = new OrderBuilder();
            List<Order> orders = new List<Order>()
            {
                builder.Random().AddItems(2).Build(),
                builder.Random().SetUser(user).Build(),
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
