using eshopAPI.Models;
using eshopAPI.Models.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.DataAccess
{
    public interface IDiscountRepository : IBaseRepository
    {
        Task<IQueryable<AdminDiscountVM>> GetAdminDiscounts();
        Task<List<Discount>> GetDiscounts();
        Task<Discount> InsertDiscount(Discount discount);
        Task<Discount> GetDiscountForItem(long itemId);
        Task<Discount> GetDiscountForCategory(long categoryId);
        Task<Discount> GetDiscountForSubCategory(long subCategoryId);
        Task<Discount> GetDiscountByID(long id);
        Task RemoveDiscount(Discount discount);
    }

    public class DiscountRepository : BaseRepository, IDiscountRepository
    {
        public DiscountRepository(ShopContext context) : base(context)
        {
        }

        public Task<IQueryable<AdminDiscountVM>> GetAdminDiscounts()
        {
            var query = Context.Discounts
                .Select(x => new AdminDiscountVM()
                {
                    ID = x.ID,
                    Name = x.Name,
                    Value = x.Value,
                    To = x.To.ToShortDateString(),
                    CategoryId = x.Category.ID,
                    CategoryName = x.Category.Name,
                    SubCategoryId = x.SubCategory.ID,
                    SubCategoryName = x.SubCategory.Name,
                    ItemId = x.Item.ID,
                    ItemName = x.Item.Name,
                    IsPercentages = x.IsPercentages
                });

            return Task.FromResult(query);
        }

        public Task<Discount> GetDiscountByID(long id)
        {
            return Context.Discounts.FirstOrDefaultAsync(i => i.ID == id);
        }

        public Task<Discount> GetDiscountForCategory(long categoryId)
        {
            return Context.Discounts.FirstOrDefaultAsync(i => i.CategoryID.HasValue && i.CategoryID.Value == categoryId);
        }

        public Task<Discount> GetDiscountForItem(long itemId)
        {
            return Context.Discounts.FirstOrDefaultAsync(i => i.ItemID.HasValue && i.ItemID.Value == itemId);
        }

        public Task<Discount> GetDiscountForSubCategory(long subCategoryId)
        {
            return Context.Discounts.FirstOrDefaultAsync(i => i.SubCategoryID.HasValue && i.SubCategoryID.Value == subCategoryId);
        }

        public Task<List<Discount>> GetDiscounts()
        {
            return Context.Discounts.Where(i => i.To == null || i.To >= DateTime.Now).ToListAsync();
        }

        public Task<Discount> InsertDiscount(Discount discount)
        {
            return Task.FromResult(Context.Discounts.Add(discount).Entity);
        }

        public Task RemoveDiscount(Discount discount)
        {
            Context.Discounts.Remove(discount);
            return Task.CompletedTask;
        }
    }
}
