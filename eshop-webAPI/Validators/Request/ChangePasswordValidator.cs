using eshopAPI.Requests.Account;
using FluentValidation;

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
                .WithMessage("New password can\'t match current password");
            RuleFor(r => r.RepeatNewPassword)
                .NotEmpty()
                .Equal(r => r.NewPassword)
                .WithMessage("New passwords don\'t match");
        }
    }
}
