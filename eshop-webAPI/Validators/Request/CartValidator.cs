using eshopAPI.Requests.Cart;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class CartValidator : AbstractValidator<CartRequest>
    {
        public CartValidator()
        {
            RuleFor(r => r.Items).NotEmpty();
        }
    }
}
