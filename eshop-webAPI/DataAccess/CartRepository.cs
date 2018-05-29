using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface ICartRepository : IBaseRepository
    {
        Task<Cart> FindByUser(string email);
        Task<Cart> FindByUserWithoutItemsData(string email);
        Task<Cart> Insert(Cart cart);
        Task RemoveCartItem(CartItem item);
        Task ClearCart(Cart cart);
    }

    public class CartRepository : BaseRepository, ICartRepository
    {
        public CartRepository(ShopContext context) : base(context)
        {
        }

        public Task<Cart> FindByUser(string email)
        {
            return Context.Carts.Include(c => c.User)
                .Include(c => c.Items).ThenInclude(i => i.Item).ThenInclude(p => p.Pictures)
                .Include(c => c.Items).ThenInclude(i => i.Item).ThenInclude(a => a.Attributes).ThenInclude(a => a.Attribute)
                .Where(c => c.User.NormalizedEmail.Equals(email.Normalize()))
                .FirstOrDefaultAsync();
        }

        public Task<Cart> FindByUserWithoutItemsData(string email)
        {
            return Context.Carts.Include(c => c.Items)
                .Where(c => c.User.NormalizedEmail.Equals(email.Normalize()))
                .FirstOrDefaultAsync();
        }

        public Task<Cart> Insert(Cart cart)
        {
            return Task.FromResult(Context.Carts.Add(cart).Entity);
        }

        public Task RemoveCartItem(CartItem item)
        {
            Context.CartItems.Remove(item);

            return Task.CompletedTask;
        }

        public Task ClearCart(Cart cart)
        {
            foreach (var cartItem in cart.Items)
            {
                RemoveCartItem(cartItem);
            }
            Context.Carts.Remove(cart);
            return Task.CompletedTask;
        }
    }
}
