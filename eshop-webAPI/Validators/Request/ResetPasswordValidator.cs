using eshopAPI.Requests.Account;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(r => r.UserId).NotEmpty();
            RuleFor(r => r.Token).NotEmpty();
            RuleFor(r => r.Password).NotEmpty().Equal(r => r.ConfirmPassword)
                .When(r => !string.IsNullOrEmpty(r.ConfirmPassword)).WithMessage("Passwords should match.");
        }
    }
}