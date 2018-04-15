using eshopAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IItemRepository
    {
        IQueryable<AdminItemVM> GetAllAdminItemVMAsQueryable();
        Item FindByID(long itemID);
        Task<Item> InsertAsync(Item item);
        void Update(Item item);
        void Save();
    }

    public class ItemRepository : BaseRepository, IItemRepository
    {
        public ItemRepository(ShopContext context) : base(context)
        {
        }

        public IQueryable<AdminItemVM> GetAllAdminItemVMAsQueryable()
        {
            var query =
                from item in Context.Items
                join category in Context.Categories on item.CategoryID equals category.ID
                select new AdminItemVM()
                {
                    Category = category.Name,
                    Name = item.Name,
                    ID = item.ID,
                    Description = item.Description,
                    Price = item.Price,
                    SKU = item.SKU
                };
            return query;
        }

        public Item FindByID(long itemID)
        {
            throw new NotImplementedException();
        }

        public async Task<Item> InsertAsync(Item item)
        {
            Item insertedItem = (await Context.Items.AddAsync(item)).Entity;
            await Context.SaveChangesAsync();
            return insertedItem;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
