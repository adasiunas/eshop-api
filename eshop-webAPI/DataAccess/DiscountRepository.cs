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
                    To = x.To,
                    CategoryName = x.Category.Name,
                    SubCategoryName = x.SubCategory.Name,

                });

            return Task.FromResult(query);
        }

        public async Task<Discount> InsertDiscount(Discount discount)
        {
            return (await Context.Discounts.AddAsync(discount)).Entity;
        }
    }
}
