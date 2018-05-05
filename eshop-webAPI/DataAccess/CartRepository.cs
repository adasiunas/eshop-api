using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface ICartRepository : IBaseRepository
    {
        Cart FindByID(long cartID);
        Task<Cart> FindByUser(string email);
        void Insert(Cart cart);
        void Update(Cart cart);
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

        public async Task<Cart> FindByUser(string email)
        {
            return await Context.Carts.Include(c => c.User).Include(c => c.Items).Where(c => c.User.NormalizedEmail.Equals(email.Normalize())).FirstOrDefaultAsync();
        }

        public void Insert(Cart cart)
        {
            Context.Carts.Add(cart);
        }

        public void Update(Cart cart)
        {
            throw new NotImplementedException();
        }
    }
}
