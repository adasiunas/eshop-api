using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IItemRepository
    {
        Task<Item> FindByID(long itemID);
        Task<Item> InsertAsync(Item item);
        IQueryable<AdminItemVM> GetAllAdminItemVMAsQueryable();
        void Update(Item item);
        void Save();
        IQueryable<ItemVM> GetAllItemsForFirstPageAsQueryable();
    }

    public class ItemRepository : BaseRepository, IItemRepository
    {
        private readonly ShopContext _context;
        public ItemRepository(ShopContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<AdminItemVM> GetAllAdminItemVMAsQueryable()
        {
            var query =
                Context.Items
                .Include(x => x.SubCategory)
                .Select(x => new AdminItemVM()
                {
                    Category = x.SubCategory.Name,
                    Name = x.Name,
                    ID = x.ID,
                    Description = x.Description,
                    Price = x.Price,
                    SKU = x.SKU
                });
            return query;
        }

        public async Task<Item> FindByID(long itemID)
        {
            return await _context.Items.Where(i => i.ID == itemID)
                .Include(i => i.Pictures)
                .Include(i => i.Attributes).ThenInclude(a => a.Attribute)
                .FirstOrDefaultAsync();
        }

        public async Task<Item> InsertAsync(Item item)
        {
            return (await Context.Items.AddAsync(item)).Entity;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(Item item)
        {
            throw new NotImplementedException();
        }
        public IQueryable<ItemVM> GetAllItemsForFirstPageAsQueryable()
        {
            var query = _context.Items
                .Where(i => i.IsDeleted.Equals(false))
                .Select(i => new ItemVM
                {
                    ID = i.ID,
                    SKU = i.SKU,
                    Name = i.Name,
                    Price = i.Price,
                    MainPicture = i.Pictures.Select(p => p.URL).FirstOrDefault(),
                    Attributes = i.Attributes.Select(a => new ItemAttributesVM
                    {
                        Name = a.Attribute.Name,
                        Value = a.Value
                    }).Take(2)
                });

            return query;
        }

    }
}
