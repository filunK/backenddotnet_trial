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
using FilunK.backenddotnet_trial.Utils;


namespace FilunK.backenddotnet_trial.Controllers
{

    /// <summary>
    ///     認証系API
    /// </sumamry>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppSettings _config ;

        public AuthenticationController(IOptions<AppSettings> configuration)
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
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            IActionResult response = Unauthorized();

            // ログイン認証
            var user = this.Authenticate(model);

            if (user != null)
            {
                // JWT生成
                var generatedToken = TokenUtil.GenerateToken(this._config.Jwt.Issuer, this._config.Jwt.Issuer, this._config.Jwt.ExpireMinutes, user, this._config.Jwt.TokenKey, SecurityAlgorithms.HmacSha256);
                var refreshToken = TokenUtil.GenerateRefreshToken(this._config.Jwt.Issuer, this._config.Jwt.Issuer, generatedToken, this._config.Jwt.RefreshTokenKey, SecurityAlgorithms.HmacSha512);
                response = Ok(new {
                    token = generatedToken,
                    refresh = refreshToken
                });
            }

            return response;
        }

        /// <summary>
        ///     認証有効性確認
        /// </summary>
        /// <remark>
        ///     JWT認証の正常性を確認します。
        ///     承認できれば200を、再認証が必要である場合は401を返します。
        /// </remark>
        ///
        [Route("confirm")]
        [Authorize]
        [HttpGet]
        public IActionResult Confirm()
        {
            return Ok();
        }



        #region 非HTTPメソッド

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

        #endregion
    }
}
