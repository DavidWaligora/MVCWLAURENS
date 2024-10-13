using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace StartspelerAPI.Helper
{
    public class Token
    {
        public static MySettings? mySettings;

        public static JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySettings!.Secret!));
            var token = new JwtSecurityToken(
            issuer: mySettings!.ValidIssuer!,
                audience: mySettings!.ValidAudience!,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
            return token;
        }
    }
}
