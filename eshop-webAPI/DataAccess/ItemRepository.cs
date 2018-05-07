using eshopAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IItemRepository : IBaseRepository
    {
        Task<Item> FindByID(long itemID);
        Task<Item> Insert(Item item);
        Task<IQueryable<AdminItemVM>> GetAllAdminItemVMAsQueryable();
        Task Update(Item item);
        Task<IQueryable<ItemVM>> GetAllItemsForFirstPageAsQueryable();
    }

    public class ItemRepository : BaseRepository, IItemRepository
    {
        public ItemRepository(ShopContext context) : base(context)
        {
        }

        public Task<IQueryable<AdminItemVM>> GetAllAdminItemVMAsQueryable()
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
            return Task.FromResult(query);
        }

        public Task<Item> FindByID(long itemID)
        {
            return Context.Items.Where(i => i.ID == itemID)
                .Include(i => i.Pictures)
                .Include(i => i.Attributes).ThenInclude(a => a.Attribute)
                .FirstOrDefaultAsync();
        }

        public async Task<Item> Insert(Item item)
        {
            return (await Context.Items.AddAsync(item)).Entity;
        }

        public Task Update(Item item)
        {
            throw new NotImplementedException();
        }
        public Task<IQueryable<ItemVM>> GetAllItemsForFirstPageAsQueryable()
        {
            var query = Context.Items
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
                    }).Take(2),
                    ItemCategory = new ItemCategoryVM
                    {
                        Name = i.SubCategory.Category.Name,
                        ID = i.SubCategory.CategoryID,
                        SubCategory = new ItemSubCategoryVM
                        {
                            ID = i.SubCategoryID,
                            Name = i.SubCategory.Name
                        }
                    }
                });
            return Task.FromResult(query);
        }

    }
}
