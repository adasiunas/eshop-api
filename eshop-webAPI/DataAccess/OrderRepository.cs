using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
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
        Task<Order> FindByOrderNumber(Guid orderNumber);
        Task Insert(Order order);
        Task<IQueryable<OrderVM>> GetAllOrdersAsQueryable(string email);
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

        public Task<Order> FindByOrderNumber(Guid orderNumber)
        {
            return Context.Orders.Where(o => o.OrderNumber == orderNumber)
                .Include(o => o.Items)
                    .ThenInclude(item => item.Item).FirstOrDefaultAsync();
        }

        public Task Insert(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<OrderVM>> GetAllOrdersAsQueryable(string email)
        {
            var query = Context.Orders
                .Where(q => q.User.NormalizedUserName.Equals(email))
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
            return Task.FromResult(query);
        }
    }
}
