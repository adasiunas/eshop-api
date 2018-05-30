using eshopAPI.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{

    public class DiscountValidator : AbstractValidator<DiscountRequest>
    {
        public DiscountValidator()
        {
            // RuleFor(r => r.Name).NotEmpty();
            RuleFor(r => r.Value).NotNull();
            RuleFor(r => r.IsPercentages).NotNull();
        }
    }
}
