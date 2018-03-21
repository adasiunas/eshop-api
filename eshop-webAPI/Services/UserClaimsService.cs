using eshopAPI.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eshopAPI.Services
{
    public interface IUserClaimsService
    {
        Task<IEnumerable<Claim>> GetUserClaims(ShopUser user);
    }

    public class UserClaimsService : IUserClaimsService
    {
        private readonly UserManager<ShopUser> _userManager;

        public UserClaimsService(UserManager<ShopUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<Claim>> GetUserClaims(ShopUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            return claims;
        }
    }
}
