using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TurtledAuth.Contexts;
using TurtledDictionary.DatabaseModels;
using static TurtledDictionary.Consts.AuthenticationJWTConst;

namespace TurtledAuth.Helpers
{
    public class AuthenticationHelper
    {
        private readonly UserStoreDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationHelper(UserStoreDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string GetRefreshToken(string userId)
        {
            var refreshToken = _context.Find<RefreshTokenDBModel>(userId)?.RefreshToken;
            if(refreshToken == null)
            {
                var newRefreshId = Guid.NewGuid().ToString();
                SafeRefreshToken(refreshToken, userId);
            }

            return refreshToken;
        }

        public void SafeRefreshToken(string refreshToken, string userId)
        {
            _context.Add(new RefreshTokenDBModel
            {
                UserId = userId,
                RefreshToken = refreshToken,
                ValidUntil = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:RefreshExpirationTimeDays"]))
            });
            _context.SaveChanges();
        }

        public bool ValidateRefreshToken(string refreshToken, string userId)
        {
            RefreshTokenDBModel tokenRecord = _context.Find<RefreshTokenDBModel>(userId);
            return tokenRecord.RefreshToken == refreshToken && tokenRecord.ValidUntil >= DateTime.UtcNow;
        }

        public Claim[] GetUserClaims(IdentityUser user)
        {
            List<Claim> claims = _context.UserClaims.Where(x => x.UserId == user.Id).Select(y => y.ToClaim()).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            return claims.ToArray();
        }
    }
}
