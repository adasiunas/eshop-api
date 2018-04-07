using eshopAPI.Requests.Account;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordValidator()
        {
            RuleFor(r => r.CurrentPassword).NotEmpty();
            RuleFor(r => r.NewPassword)
                .NotEmpty()
                .NotEqual(r => r.CurrentPassword)
                .WithMessage("New password and current password match!");
        }
    }
}
