using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Services
{
    public interface IDiscountService
    {
        void CalculateDiscountsForItems(IEnumerable<ItemVM> items, IEnumerable<Discount> discounts);
        void CalculateDiscountsForItems(IEnumerable<CartItemVM> items, IEnumerable<Discount> discounts);
    }
    public class DiscountService : IDiscountService
    {
        public void CalculateDiscountsForItems(IEnumerable<ItemVM> items, IEnumerable<Discount> discounts)
        {
            foreach (var item in items)
            {
                item.Discount = Calculate(discounts, item.Price, item.ID, item.Category.ID, item.SubCategory?.ID);
            }
        }

        public void CalculateDiscountsForItems(IEnumerable<CartItemVM> items, IEnumerable<Discount> discounts)
        {
            foreach (var item in items)
            {
                item.Discount = Calculate(discounts, item.Price, item.ID, item.CategoryID, item.SubCategoryID);
            }
        }

        private decimal Calculate(IEnumerable<Discount> discounts, decimal price, long itemId, long categoryId, long? subCategoryId)
        {
            var discount = discounts.Where(d => d.ItemID.HasValue && d.ItemID.Value == itemId).FirstOrDefault();
            if (discount != null)
            {
                if (discount.IsPercentages)
                {
                    return Math.Round((100 - discount.Value) * (price / 100), 2);
                }
                return price - discount.Value;
            }

            discount = discounts.Where(d => d.SubCategoryID.HasValue && d.SubCategoryID.Value == subCategoryId).FirstOrDefault();
            if (discount != null)
            {
                if (discount.IsPercentages)
                {
                    return Math.Round((100 - discount.Value) * (price / 100), 2);
                }
                return price - discount.Value;
            }

            discount = discounts.Where(d => d.CategoryID.HasValue && d.CategoryID.Value == categoryId).FirstOrDefault();
            if (discount != null)
            {
                if (discount.IsPercentages)
                {
                    return Math.Round((100 - discount.Value) * (price / 100), 2);
                }
                return price - discount.Value;
            }

            return 0;
        }
    }
}
