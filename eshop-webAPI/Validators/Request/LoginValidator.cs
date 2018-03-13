using eshopAPI.Requests;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(r => r.Password).NotEmpty();
            RuleFor(r => r.Username).NotEmpty().EmailAddress();
        }
    }
}