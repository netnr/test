using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Netnr.AuthFailed.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text;

namespace Netnr.AuthFailed.Services
{
    /// <summary>
    /// 身份
    /// </summary>
    public class IdentityService
    {
        public static AuthorizationBaseModel Get(HttpContext context) => ClaimsParse(context.User);

        public static async Task<string> Set(HttpContext context, AuthorizationBaseModel model, bool remember = true)
        {
            var claims = ClaimsBuild(model);

            //add cookie auth (hangfire)
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            var principal = new ClaimsPrincipal(identity);
            var authProp = new AuthenticationProperties();
            if (remember)
            {
                authProp.IsPersistent = true;
                authProp.AllowRefresh = true; //自动刷新续期
                authProp.IssuedUtc = model.IssuedUtc;
                authProp.ExpiresUtc = model.ExpiresUtc;
            }
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProp);

            //add jwt auth
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.Empty.ToString()));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            var jwtToken = new JwtSecurityToken(
                issuer: "Netnr",
                audience: model.UserAccount,
                claims: claims,
                notBefore: model.IssuedUtc,
                expires: model.ExpiresUtc,
                signingCredentials: signingCredentials);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return accessToken;
        }

        /// <summary>
        /// 设置（更新）授权用户
        /// </summary>
        /// <param name="context"></param>
        /// <param name="db"></param>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public static async Task<string> Set(HttpContext context, BaseUser userEntity)
        {
            var model = new AuthorizationBaseModel
            {
                UserId = userEntity.UserId,
                UserAccount = userEntity.UserAccount,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            };

            //登录
            var accessToken = await Set(context, model);
            return accessToken;
        }

        public static IEnumerable<Claim> ClaimsBuild(AuthorizationBaseModel model)
        {
            var claims = new Claim[]
            {
                new(ClaimTypes.NameIdentifier, model.UserId.ToString()),
                new(ClaimTypes.Name, model.UserAccount),

                new(ClaimTypes.Expiration, model.IssuedUtc.ToString()),
                new(ClaimTypes.Expired, model.ExpiresUtc.ToString())
            };

            return claims;
        }

        public static AuthorizationBaseModel ClaimsParse(ClaimsPrincipal user)
        {
            if (user.Identity?.IsAuthenticated == true)
            {
                var model = new AuthorizationBaseModel
                {
                    UserId = Convert.ToInt64(user.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                    UserAccount = user.FindFirst(ClaimTypes.Name)?.Value,
                };

                if (model.UserId > 0)
                {
                    model.IssuedUtc = DateTime.Parse(user.FindFirst(ClaimTypes.Expiration)?.Value);
                    model.ExpiresUtc = DateTime.Parse(user.FindFirst(ClaimTypes.Expired)?.Value);

                    return model;
                }
            }

            return null;
        }

    }
}
