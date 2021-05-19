using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Auth.Common.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Common.Services
{
    internal class JwtTokenGetterService : ITokenGetterService
    {

        private readonly AuthConfiguration _authConfiguration;

        public JwtTokenGetterService(IOptions<AuthConfiguration> authConfiguration)
        {
            _authConfiguration = authConfiguration.Value;
        }

        public string GetToken(string userId, DateTime @from, IEnumerable<string> roles, IDictionary<string, string> additionalClaims = null)
        {
            throw new NotImplementedException();
        }

        private ClaimsIdentity GetIdentity(string userId, IEnumerable<string> roles, IDictionary<string, string> additionalClaims)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userId),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims.Select(x => new Claim(x.Key, x.Value)));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "Token");
            return claimsIdentity;
        }

        private string GetJwtToken(DateTime from, IEnumerable<Claim> claims)
        {
            var expires = from.Add(TimeSpan.FromMinutes(_authConfiguration.LifeTime ?? 1440));

            var authLoginKey = _authConfiguration.GetSymmetricSecurityKey();

            var token = new JwtSecurityToken(
                issuer: _authConfiguration.Issuer,
                audience: _authConfiguration.Audience,
                expires: expires,
                claims: claims,
                signingCredentials: new SigningCredentials(authLoginKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
