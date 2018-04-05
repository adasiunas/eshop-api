using Microsoft.AspNetCore.Mvc;

namespace eshopAPI.Helpers
{
    public static class UrlExtensions
    {
        
        public static string EmailConfirmationLink(string userId, string code, string domain)
        {
            return domain + "/confirmaccount?UserId=" + userId + "&Code=" + code; 
        }

        public static string ResetPasswordLink(string userId, string code, string domain)
        {
            return domain+ "resetpassword?Id=" +userId + "&token=" + code;
        }
    }
}
