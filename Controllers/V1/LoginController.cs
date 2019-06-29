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

        // POST api/values
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
                var generatedToken = this.GenerateToken(user);
                response = Ok(new {
                    token = generatedToken
                });
            }

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
        private string GenerateToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config.Jwt.Key));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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



    }
}
