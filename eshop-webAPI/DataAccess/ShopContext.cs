using eshopAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eshopAPI.DataAccess

{
    public class ShopContext : IdentityDbContext<ShopUser>
    {
        public ShopContext(DbContextOptions<ShopContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; }
        public DbSet<Cart> Carts { get; }
        public DbSet<CartItem> CartItems { get; }
        public DbSet<Item> Items { get; }
        public DbSet<Order> Orders { get; }
        public DbSet<OrderItem> OrderItems { get; }
        public DbSet<Category> Categories { get; }
        public DbSet<SubCategory> SubCategories { get; }
        public DbSet<Attribute> Attributes { get; }
        public DbSet<ItemPicture> ItemPictures { get; }
    }
}
