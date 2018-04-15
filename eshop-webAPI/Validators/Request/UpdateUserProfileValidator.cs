using eshopAPI.Requests.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class UpdateUserProfileValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserProfileValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .Matches(@"^[A-Za-z]+$")
                .WithMessage("Name can only contain letters.");
            RuleFor(r => r.Surname)
                .NotEmpty()
                .Matches(@"^[A-Za-z]+$")
                .WithMessage("Surname can only contain letters.");
            RuleFor(r => r.Phone)
                .NotEmpty()
                .Matches(@"^[0-9]+$")
                .WithMessage("Phone can only contain digits.");
            RuleFor(r => r.Address).SetValidator(new AddressValidator());
        }
    }
}
