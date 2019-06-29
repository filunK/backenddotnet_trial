using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Security.Claims;

using FilunK.backenddotnet_trial.Models;
using FilunK.backenddotnet_trial.Models.Configure;
using FilunK.backenddotnet_trial.Models.Authentication;

namespace FilunK.backenddotnet_trial.Controllers
{

    /// <summary>
    ///     ログインAPI
    /// </sumamry>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppSettings _config ;

        public LoginController(IOptions<AppSettings> configuration)
        {
            this._config = configuration.Value;
        }

        /// <summary>
        ///     ログイン
        /// </summary>
        /// <remark>
        ///     このAPIでは以下のJSONを<c>POST</c>メソッドで受け取ります。
        ///     <code>
        ///         {
        ///             username: string
        ///             password: string
        ///          }
        ///     </code>
        ///     ログインに成功した場合、JWTを返却します。
        /// </remark>
        ///
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] LoginModel model)
        {
            IActionResult response = Unauthorized();

            // ログイン認証
            var user = this.Authenticate(model);

            if (user != null)
            {
                // JWT生成
                var generatedToken = this.GenerateToken(user, this._config.Jwt.TokenKey, SecurityAlgorithms.HmacSha256);
                var refreshToken = this.GenerateRefreshToken(generatedToken, this._config.Jwt.RefreshTokenKey, SecurityAlgorithms.HmacSha512);
                response = Ok(new {
                    token = generatedToken,
                    refresh = refreshToken
                });
            }

            return response;
        }

        /// <summary>
        ///     ログイン
        /// </summary>
        /// <remark>
        ///     このAPIでは以下のJSONを<c>PUT</c>メソッドで受け取ります。
        ///     <code>
        ///         {
        ///             accessToken: string
        ///             refreshToken: string
        ///          }
        ///     </code>
        ///     成功した場合、再生成したJWTを返却します。
        /// </remark>
        ///
        [AllowAnonymous]
        [HttpPut]
        public IActionResult Put([FromBody] RefreshRequestModel model)
        {
            IActionResult response = Unauthorized();

            return response;
        }


        /// <summary>
        ///     ユーザ情報を取得する
        /// </summary>
        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = new UserModel{
                UserName = login.Username,
            };

            user.MailAddress = "dummy@dummy.com";
            user.BirthDate = DateTime.Now;

            return user;
        }

        /// <summary>
        ///     JWTを生成する
        /// </summary>
        private string GenerateToken(UserModel user,string tokenString, string algorithm)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenString));
            var credential = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer: this._config.Jwt.Issuer,
                audience: this._config.Jwt.Issuer,
                claims: new[] {
                    new Claim("username", user.UserName),
                    new Claim("mailaddress", user.MailAddress),
                    new Claim("birthday", user.BirthDate.ToLongDateString()),
                },
                expires: DateTime.Now.AddMinutes(this._config.Jwt.ExpireMinutes),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateRefreshToken(string jwt, string tokenString, string algorithm)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenString));
            var credential = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                issuer: this._config.Jwt.Issuer,
                audience: this._config.Jwt.Issuer,
                claims: new[] {
                    new Claim("token", jwt),
                },
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
