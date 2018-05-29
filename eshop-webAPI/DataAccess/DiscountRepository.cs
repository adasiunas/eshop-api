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
        Task<IQueryable<AdminDiscountVM>> GetAllValidDiscounts();
        Task<Discount> InsertDiscount(Discount discount);
    }

    public class DiscountRepository : BaseRepository, IDiscountRepository
    {
        public DiscountRepository(ShopContext context) : base(context)
        {
        }

        public Task<IQueryable<AdminDiscountVM>> GetAllValidDiscounts()
        {
            var query = Context.Discounts
                //.Where(x => x.To < DateTime.Now && x.To != null)
                .Select(x => new AdminDiscountVM()
                {
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

        public async Task<Discount> InsertDiscount(Discount discount)
        {
            return (await Context.Discounts.AddAsync(discount)).Entity;
        }
    }
}
