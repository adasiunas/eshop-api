using eshop_webAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace eshopAPI.Helpers
{
    public static class UrlExtensions
    {
        
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string domain)
        {
            return domain + "/confirmaccount?UserId=" + userId + "&Code=" + code; 
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AccountController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
