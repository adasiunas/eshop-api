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
        Task<Cart> FindByID(long cartID);
        Task<Cart> FindByUser(string email);
        Task Insert(Cart cart);
        Task Update(Cart cart);
    }

    public class CartRepository : BaseRepository, ICartRepository
    {
        public CartRepository(ShopContext context) : base(context)
        {
        }

        public Task<Cart> FindByID(long cartID)
        {
            throw new NotImplementedException();
        }

        public Task<Cart> FindByUser(string email)
        {
            return Context.Carts.Include(c => c.User).Include(c => c.Items).Where(c => c.User.NormalizedEmail.Equals(email.Normalize())).FirstOrDefaultAsync();
        }

        public Task Insert(Cart cart)
        {
            return Context.Carts.AddAsync(cart);
        }

        public Task Update(Cart cart)
        {
            throw new NotImplementedException();
        }
    }
}
