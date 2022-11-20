using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using TurtledAuth.Contexts;
using TurtledAuth.Helpers;
using TurtledAuth.Models.AuthenticationResources.Communication;
using TurtledDictionary.Resources.Authentication;
using static TurtledDictionary.Consts.AuthenticationJWTConst;

namespace TurtledAuth.Services
{
    public class AuthenticationJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly int _expirationTime;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly UserStoreDbContext _context;
        private readonly AuthenticationHelper _authenticationHelper;

        public AuthenticationJWTService(IConfiguration configuration, UserStoreDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _expirationTime = int.Parse(configuration["JWT:ExpirationTime"]);
            _tokenHandler = new JwtSecurityTokenHandler();
            _authenticationHelper = new AuthenticationHelper(_context, _configuration);
        }



        public AuthenticationJWTResponse GetToken(IdentityUser user, string? refreshToken = null)
        {
            DateTime expirationDate = DateTime.UtcNow.AddSeconds(_expirationTime);
            var token = CreateAccessToken(_authenticationHelper.GetUserClaims(user), CreateSigningCredentials(), expirationDate);
            var newRefreshToken = refreshToken ?? _authenticationHelper.GetRefreshToken(user.Id);

            return new AuthenticationJWTResponse
            {
                Token = _tokenHandler.WriteToken(token),
                RefreshToken = newRefreshToken
            };
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                ValidateLifetime = false
            };
            SecurityToken validatedToken;
            var principal = _tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            var validatedJWTToken = validatedToken as JwtSecurityToken;
            
            if(validatedJWTToken == null)
            {
                throw new SecurityTokenException();
            }

            return principal;
        }

        public AuthenticationJWTResponse GetValidateRefreshToken(IdentityUser user, string refreshToken, ClaimsPrincipal principal)
        {           
            if(!_authenticationHelper.ValidateRefreshToken(refreshToken, user.Id))
            {
                throw new SecurityTokenException();
            }

            return GetToken(user, refreshToken);
        }

        private JwtSecurityToken CreateAccessToken(Claim[] claims, SigningCredentials credentials, DateTime expiration) =>
            new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        private SigningCredentials CreateSigningCredentials() =>
            new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["JWT:Key"])
        ),
                SecurityAlgorithms.HmacSha256
        );
    }
}
