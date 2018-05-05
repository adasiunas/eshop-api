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
        void Insert(Item item);
        void Update(Item item);
        IQueryable<ItemVM> GetAllItemsForFirstPageAsQueryable();
    }

    public class ItemRepository : BaseRepository, IItemRepository
    {
        public ItemRepository(ShopContext context) : base(context)
        {
        }

        public async Task<Item> FindByID(long itemID)
        {
            return await Context.Items.Where(i => i.ID == itemID)
                .Include(i => i.Pictures)
                .Include(i => i.Attributes).ThenInclude(a => a.Attribute)
                .FirstOrDefaultAsync();
        }

        public void Insert(Item item)
        {
            throw new NotImplementedException();
        }

        public void Update(Item item)
        {
            throw new NotImplementedException();
        }
        public IQueryable<ItemVM> GetAllItemsForFirstPageAsQueryable()
        {
            var query = Context.Items
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
