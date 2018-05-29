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

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<AttributeValue> AttributeValue { get; set; }
        public DbSet<ItemPicture> ItemPictures { get; set; }
        public DbSet<UserFeedbackEntry> UserFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasSequence<int>("OrderNumbers", schema: "shared")
                .StartsAt(100000)
                .IncrementsBy(1);

            //Once a sequence is introduced, you can use it to generate values for properties in your model. For example, you can use Default Values to insert the next value from the sequence.
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNumber)
                .HasDefaultValueSql("NEXT VALUE FOR shared.OrderNumbers");
        }
    }
}
