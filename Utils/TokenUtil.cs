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
                },
                expires: DateTime.Now.AddMinutes(expires),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        /// <summary>
        ///     リフレッシュトークンを生成する
        /// </summary>
        public static string GenerateRefreshToken(string issuer, string audience, string jwt, string tokenString, double expireDays, string algorithm)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenString));
            var credential = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] {
                    new Claim("token", jwt),
                },
                expires: DateTime.Now.AddDays(expireDays),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        ///     リフレッシュトークンをデコードする
        /// </summary>
        public static UserModel DecodeRefreshToken(string refreshToken, string refreshTokenKeyString, string accessTokenKeyString, string issuer)
        {

            var refKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenKeyString));
            var accKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessTokenKeyString));

            var handler = new JwtSecurityTokenHandler();
            var refreshTokenValidationParams = new TokenValidationParameters
            {
                ValidAudience = issuer,
                ValidIssuer = issuer,
                ValidateLifetime = true,
                IssuerSigningKey = refKey
            };

            try
            {
                var claims = handler.ValidateToken(refreshToken, refreshTokenValidationParams, out var token);

                var resolvedToken = (
                    from claim in claims.Claims
                    where claim.Type == "token"
                    select claim
                ).FirstOrDefault();

                // refreshTokenに含まれていたAccessTokenを解析する。
                var accessTokenValidationParams = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidIssuer = issuer,
                    ValidateLifetime = false,
                    IssuerSigningKey = accKey
                };

                var resolvedClaim = handler.ValidateToken(resolvedToken.Value, accessTokenValidationParams, out var reresolvedToken);

                // claimからUserModelを構築
                var model = new UserModel();
                foreach (var claim in resolvedClaim.Claims)
                {
                    switch (claim.Type)
                    {
                        case "username":
                            model.UserName = claim.Value;
                            break;
                        case "mailaddress":
                            model.MailAddress = claim.Value;
                            break;
                        default:
                            break;
                    }

                }

                return model;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}

