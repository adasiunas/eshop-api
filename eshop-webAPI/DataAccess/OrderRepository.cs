using eshopAPI.Models;
using System;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IOrderRepository : IBaseRepository
    {
        Task<Order> FindByID(long orderID);
        Task<Order> FindByOrderNumber(Guid orderNumber);
        Task Insert(Order order);
        Task Update(Order order);
    }

    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ShopContext context) : base(context)
        {
        }

        public Task<Order> FindByID(long orderID)
        {
            throw new NotImplementedException();
        }

        public Task<Order> FindByOrderNumber(Guid orderNumber)
        {
            throw new NotImplementedException();
        }

        public Task Insert(Order order)
        {
            throw new NotImplementedException();
        }

        public Task Update(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
