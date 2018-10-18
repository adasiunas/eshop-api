using eshopAPI.Models;
using eshopAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Services.DiscountServiceTests
{
    public class Calculate
    {
        decimal _initialPrice = 1000;
        int _discountValue = 10;
        decimal percentageDiscount;
        decimal flatDiscount;
        IDiscountService _discountService;
        List<Discount> _discounts;          // Same discounts used for all test cases

        public Calculate()
        {
            _discounts = InitializeDiscounts();
            _discountService = new DiscountService();
            percentageDiscount = Math.Round((100 - _discountValue) * (_initialPrice / 100), 2);
            flatDiscount = _initialPrice - _discountValue;
        }

        [Fact]
        public void CalculateDiscountItemVM()
        {
            List<ItemVM> items = new List<ItemVM>();
            items.Add(CreateItemVm(1, 1));
            items.Add(CreateItemVm(2, 1));
            items.Add(CreateItemVm(3, 1, 1));
            items.Add(CreateItemVm(3, 1, 2));
            items.Add(CreateItemVm(3, 1, 3));
            items.Add(CreateItemVm(3, 2, 3));
            List<decimal> expectedDiscounts = (new decimal[6] {
                percentageDiscount, flatDiscount,
                percentageDiscount, flatDiscount,
                percentageDiscount, flatDiscount,}).ToList();
            _discountService.CalculateDiscountsForItems(items, _discounts);

           Assert.All(Enumerable.Range(0, 6), i => Assert.Equal(expectedDiscounts.ElementAt(i), items.ElementAt(i).Discount));
        }

        [Fact]
        public void CalculateDiscountCartItemVM()
        {
            List<CartItemVM> items = new List<CartItemVM>();
            items.Add(CreateCartItemVm(1, 1));
            items.Add(CreateCartItemVm(2, 1));
            items.Add(CreateCartItemVm(3, 1, 1));
            items.Add(CreateCartItemVm(3, 1, 2));
            items.Add(CreateCartItemVm(3, 1, 3));
            items.Add(CreateCartItemVm(3, 2, 3));
            List<decimal> expectedDiscounts = (new decimal[6] {
                percentageDiscount, flatDiscount,
                percentageDiscount, flatDiscount,
                percentageDiscount, flatDiscount,}).ToList();
            _discountService.CalculateDiscountsForItems(items, _discounts);

            Assert.All(Enumerable.Range(0, 6), i => Assert.Equal(expectedDiscounts.ElementAt(i), items.ElementAt(i).Discount));
        }

        [Fact]
        public void CalculateDiscountCheapItems()
        {
            decimal _price = 5;
            List<CartItemVM> items = new List<CartItemVM>();
            items.Add(CreateCartItemVm(2, 1));
            items.Add(CreateCartItemVm(3, 1, 2));
            items.Add(CreateCartItemVm(3, 2, 3));
            items.ForEach(o => o.Price = _price);
            _discountService.CalculateDiscountsForItems(items, _discounts);

            Assert.All(Enumerable.Range(0, 3), i => Assert.Equal(0, items.ElementAt(i).Discount));
        }

        [Fact]
        public void CalculateDiscountWithNoDiscount()
        {
            List<CartItemVM> items = new List<CartItemVM>();
            items.Add(CreateCartItemVm(4, 4, 4));
            _discountService.CalculateDiscountsForItems(items, _discounts);

            Assert.Equal(0, items.First().Discount);
        }

        List<Discount> InitializeDiscounts()
        {
            List<Discount> discounts = new List<Discount>();
            discounts.Add(new Discount
            {
                ID = 1,
                ItemID = 1,
                IsPercentages = true,
                Value = _discountValue
            });
            discounts.Add(new Discount
            {
                ID = 2,
                ItemID = 2,
                IsPercentages = false,
                Value = _discountValue
            });
            discounts.Add(new Discount
            {
                ID = 3,
                CategoryID = 1,
                IsPercentages = true,
                Value = _discountValue
            });
            discounts.Add(new Discount
            {
                ID = 4,
                CategoryID = 2,
                IsPercentages = false,
                Value = _discountValue
            });
            discounts.Add(new Discount
            {
                ID = 5,
                SubCategoryID = 1,
                IsPercentages = true,
                Value = _discountValue
            });
            discounts.Add(new Discount
            {
                ID = 6,
                SubCategoryID = 2,
                IsPercentages = false,
                Value = _discountValue
            });
            return discounts;
        }

        ItemVM CreateItemVm(int id, int categoryId, int? subcategoryId = null)
        {
            ItemVM item = new ItemVM
            {
                ID = id,
                Category = new ItemCategoryVM
                {
                    ID = categoryId
                },
                Price = _initialPrice
            };
            if (subcategoryId != null)
            {
                item.SubCategory = new ItemSubCategoryVM
                {
                    ID = subcategoryId.Value
                };
            }
            return item;
        }

        CartItemVM CreateCartItemVm(int id, int categoryId, int? subcategoryId = null)
        {
            CartItemVM item = new CartItemVM
            {
                ItemID = id,
                CategoryID = categoryId,
                SubCategoryID = subcategoryId,
                Price = _initialPrice
            };
            return item;
        }
    }
}
