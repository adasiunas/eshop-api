using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eshopAPI.Services
{
    public interface ITokenGenerator
    {
        JwtSecurityToken GenerateToken(IEnumerable<Claim> claims);
    }
    public class TokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public TokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken GenerateToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken
            (
                issuer: _configuration["Token:Issuer"],
                audience: _configuration["Token:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"])), SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
