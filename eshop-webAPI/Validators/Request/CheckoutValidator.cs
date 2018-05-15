using eshopAPI.Requests;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class CheckoutValidator : AbstractValidator<CheckoutRequest>
    {
        public CheckoutValidator()
        {
            RuleFor(r => r.Cvv).NotEmpty();
            RuleFor(r => r.Exp_Month).NotEmpty();
            RuleFor(r => r.Exp_Year).NotEmpty();
            RuleFor(r => r.Holder).NotEmpty();
            RuleFor(r => r.Number).NotEmpty();
            RuleFor(r => r.Address).SetValidator(new AddressValidator());
        }
    }
}