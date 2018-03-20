using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface ICartRepository
    {
        Cart FindByID(long cartID);
        Cart FindByUser(long userID);
        void Insert(Cart cart);
        void Update(Cart cart);
        void Save();
    }

    public class CartRepository : BaseRepository, ICartRepository
    {
        public CartRepository(ShopContext context) : base(context)
        {
        }

        public Cart FindByID(long cartID)
        {
            throw new NotImplementedException();
        }

        public Cart FindByUser(long userID)
        {
            throw new NotImplementedException();
        }

        public void Insert(Cart cart)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(Cart cart)
        {
            throw new NotImplementedException();
        }
    }
}
