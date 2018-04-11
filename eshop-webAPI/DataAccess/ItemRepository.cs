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
        Item FindByID(long itemID);
        void Insert(Item item);
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

        public Item FindByID(long itemID)
        {
            throw new NotImplementedException();
        }



        public void Insert(Item item)
        {
            throw new NotImplementedException();
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
