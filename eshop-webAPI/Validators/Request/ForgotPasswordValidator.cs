using eshopAPI.Requests.Account;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class ForgotPasswordValidator: AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(r => r.Email).NotEmpty().EmailAddress();
        }
    }
}