using eshopAPI.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Validators.Request
{
    public class RoleChangeValidator : AbstractValidator<RoleChangeRequest>
    {
        public RoleChangeValidator()
        {
            RuleFor(r => r.Email).NotEmpty();
            RuleFor(r => r.Role).NotEmpty();
        }
    }
}
