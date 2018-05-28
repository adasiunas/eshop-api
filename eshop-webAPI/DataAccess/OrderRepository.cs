using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Models.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IOrderRepository : IBaseRepository
    {
        Task<Order> FindByID(long orderID);
        Task<Order> FindByOrderNumber(long orderNumber);
        Task<Order> Insert(Order order);
        Task<IQueryable<OrderVM>> GetAllOrdersAsQueryable(string email);
        Task<IQueryable<AdminOrderVM>> GetAllAdminOrdersAsQueryable();
    }

    public class OrderRepository : BaseRepository, IOrderRepository
    {

        public OrderRepository(ShopContext context) : base(context)
        {
        }

        public Task<Order> FindByID(long orderID)
        {
            return  Context.Orders
                .Where(o => o.ID == orderID)
                .Include(o => o.Items)
                    .ThenInclude(item => item.Item).FirstOrDefaultAsync();
        }

        public Task<Order> FindByOrderNumber(long orderNumber)
        {
            return Context.Orders.Where(o => o.OrderNumber == orderNumber)
                .Include(o => o.Items)
                    .ThenInclude(item => item.Item).FirstOrDefaultAsync();
        }

        public async Task<Order> Insert(Order order)
        {
            return (await Context.Orders.AddAsync(order)).Entity;
        }
        public Task<IQueryable<AdminOrderVM>> GetAllAdminOrdersAsQueryable()
        {
            var query = Context.Orders
                .Select(o => new AdminOrderVM()
                {
                    ID = o.ID,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.GetDescription(),
                    TotalPrice = o.Items.Select(i => i.Price * i.Count).Sum(),
                    UserEmail = o.User.NormalizedEmail,
                    DeliveryAddress = o.DeliveryAddress
                });

            return Task.FromResult(query);
        }

        public Task<IQueryable<OrderVM>> GetAllOrdersAsQueryable(string email)
        {
            var query = Context.Orders
                .Select(o =>
                    new OrderVM
                    {
                        ID = o.ID,
                        OrderNumber = o.OrderNumber,
                        Status = o.Status.GetDescription(),
                        TotalPrice = o.Items.Select(i => i.Price * i.Count).Sum(),
                        CreateDate = o.CreateDate.ToShortDateString(),
                        Items = o.Items.Select(i => new OrderItemVM {
                            ID = i.ID,
                            ItemID = i.ItemID,
                            Price = i.Price,
                            Count = i.Count,
                            Name = i.Item.Name,
                            SKU = i.Item.SKU
                        }),
                        DeliveryAddress = o.DeliveryAddress
                    });

            var tt = query.ToList();
            return Task.FromResult(query);
        }
    }
}
