using System.Net;
using eshopAPI.Requests;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class RegistrationValidator : AbstractValidator<RegisterRequest>
    {
        public RegistrationValidator()
        {
            RuleFor(r => r.Username).NotEmpty().EmailAddress();
            RuleFor(r => r.Password).NotEmpty().Equal(r => r.ConfirmPassword)
                .When(r => !string.IsNullOrEmpty(r.ConfirmPassword)).WithMessage("Passwords should match.");
        }
    }
}