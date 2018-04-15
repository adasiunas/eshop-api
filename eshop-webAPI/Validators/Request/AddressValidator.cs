using eshopAPI.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .Matches(@"^[A-Za-z]+$")
                .WithMessage("Name can only contain letters.");
            RuleFor(r => r.Surname)
                .NotEmpty()
                .Matches(@"^[A-Za-z]+$")
                .WithMessage("Surname can only contain letters.");
            RuleFor(r => r.Street).NotEmpty();
            RuleFor(r => r.City).NotEmpty();
            RuleFor(r => r.Country).NotEmpty();
            RuleFor(r => r.Postcode).NotEmpty();
        }
    }
}
