using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface ICartRepository : IBaseRepository
    {
        Task<Cart> FindByID(long cartID);
        Task<Cart> FindByUser(string email);
        Task<Cart> FindByUserWithoutItemsData(string email);
        Task Insert(Cart cart);
        Task Update(Cart cart);
        Task RemoveCartItem(CartItem item);
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
            return Context.Carts.Include(c => c.User)
                .Include(c => c.Items).ThenInclude(i => i.Item).ThenInclude(p => p.Pictures)
                .Include(c => c.Items).ThenInclude(i => i.Item).ThenInclude(a => a.Attributes).ThenInclude(a => a.Attribute)
                .Where(c => c.User.NormalizedEmail
                    .Equals(email.Normalize()))
                .FirstOrDefaultAsync();
        }

        public Task<Cart> FindByUserWithoutItemsData(string email)
        {
            return Context.Carts.Include(c => c.Items)
                .Where(c => c.User.NormalizedEmail.Equals(email.Normalize()))
                .FirstOrDefaultAsync();
        }

        public Task Insert(Cart cart)
        {
            return Context.Carts.AddAsync(cart);
        }

        public Task RemoveCartItem(CartItem item)
        {
            Context.CartItems.Remove(item);

            return Task.CompletedTask;
        }

        public Task Update(Cart cart)
        {
            throw new NotImplementedException();
        }
    }
}
