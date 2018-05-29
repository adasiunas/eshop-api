using eshopAPI.Models;
using eshopAPI.Requests.Order;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class ChangeOrderStatusValidator : AbstractValidator<ChangeOrderStatusRequest>
    {
        public ChangeOrderStatusValidator()
        {
            RuleFor(r => r.ID)
                .NotEmpty()
                .GreaterThan(0);
            RuleFor(r => r.Status)
                .NotEmpty();
        }
    }
}
