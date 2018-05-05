using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IOrderRepository : IBaseRepository
    {
        Order FindByID(long orderID);
        Order FindByOrderNumber(Guid orderNumber);
        void Insert(Order order);
        void Update(Order order);
    }

    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(ShopContext context) : base(context)
        {
        }

        public Order FindByID(long orderID)
        {
            throw new NotImplementedException();
        }

        public Order FindByOrderNumber(Guid orderNumber)
        {
            throw new NotImplementedException();
        }

        public void Insert(Order order)
        {
            throw new NotImplementedException();
        }

        public void Update(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
