using eshopAPI.Requests;
using FluentValidation;

namespace eshopAPI.Validators.Request
{
    public class UserFeedbackRequestValidator : AbstractValidator<UserFeedbackRequest>
    {
        public UserFeedbackRequestValidator()
        {
            RuleFor(r => r.Message).NotEmpty();
            RuleFor(r => r.Rating).NotEmpty();
        }
    }
}