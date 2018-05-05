using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Requests.Cart
{
    public class CartItemRequest
    {
        public long ItemID { get; set; }
        public int Count { get; set; }
    }
}
