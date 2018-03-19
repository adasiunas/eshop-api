using eshopAPI.Requests.Account;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class ConfirmAccountValidator : AbstractValidator<ConfirmUserRequest>
    {
        public ConfirmAccountValidator()
        {
            RuleFor(r => r.UserId).NotEmpty();
            RuleFor(r => r.Code).NotEmpty();
        }
    }
}