using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;

using FilunK.backenddotnet_trial.Models;

namespace FilunK.backenddotnet_trial.Utils
{

    /// <summary>
    ///     トークンユーティリティ
    /// </summary>
    public static class TokenUtil
    {

        /// <summary>
        ///     JWTを生成する
        /// </summary>
        public static string GenerateToken(string issuer, string audience, double expires, UserModel user, string tokenString, string algorithm)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenString));
            var credential = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] {
                    new Claim("username", user.UserName),
                    new Claim("mailaddress", user.MailAddress),
                    new Claim("birthday", user.BirthDate.ToLongDateString()),
                },
                expires: DateTime.Now.AddMinutes(expires),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        ///     リフレッシュトークンを生成する
        /// </summary>
        public static string GenerateRefreshToken(string issuer, string audience, string jwt, string tokenString, string algorithm)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenString));
            var credential = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] {
                    new Claim("token", jwt),
                },
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}

