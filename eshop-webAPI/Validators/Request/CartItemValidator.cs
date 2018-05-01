using eshopAPI.Requests.Cart;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class CartItemValidator : AbstractValidator<CartItemRequest>
    {
        public CartItemValidator()
        {
            RuleFor(r => r.ItemID)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.Count)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
