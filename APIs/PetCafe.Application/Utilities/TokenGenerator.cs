using Microsoft.IdentityModel.Tokens;
using PetCafe.Application.Models.AuthModels;
using PetCafe.Domain.Constants;
using PetCafe.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetCafe.Application.Utilities
{
    public static class TokenGenerator
    {
        public static string GenerateToken(Account account, AppSettings _appSettings)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JWTOptions.SecretKey);

            var target_id = account.Role == RoleConstants.CUSTOMER ? account.Customer!.Id : account.Employee!.Id;
            var claimsList = new List<Claim>()
            {
                new(ClaimTypes.Email, account.Email!),
                new(ClaimTypes.NameIdentifier, target_id.ToString()),
                new(ClaimTypes.Role, account.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _appSettings.JWTOptions.Audience,
                Issuer = _appSettings.JWTOptions.Issuer,
                Subject = new ClaimsIdentity(claimsList),
                Expires = DateTime.UtcNow.AddDays(1000),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public static string RevokeToken(RevokeModel model, AppSettings _appSettings)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWTOptions.SecretKey));

                tokenHandler.ValidateToken(model.RefreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _appSettings.JWTOptions.Issuer,
                    ValidAudience = _appSettings.JWTOptions.Audience,
                    IssuerSigningKey = securityKey
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = jwtToken.Claims;

                var newToken = new JwtSecurityToken(
                    issuer: _appSettings.JWTOptions.Issuer,
                    audience: _appSettings.JWTOptions.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(10),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                return tokenHandler.WriteToken(newToken);
            }
            catch (SecurityTokenException ex)
            {
                throw new SecurityTokenException("Invalid or tampered token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to refresh token.", ex);
            }
        }
    }
}
