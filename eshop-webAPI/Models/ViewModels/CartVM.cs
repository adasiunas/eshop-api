using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Models.ViewModels
{
    public class CartVM
    {
        public long ID { get; set; }
        public IEnumerable<CartItemVM> Items { get; set; }
    }

    public static class CartVMExtensions
    {
        public static CartVM GetCartVM(this Cart cart)
        {
            return new CartVM
            {
                ID = cart.ID,
                Items = cart.Items.Select(i => i.GetCartItemVM()).ToList()
            };
        }
    }
}
