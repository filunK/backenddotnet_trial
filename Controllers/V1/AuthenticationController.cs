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
using Microsoft.EntityFrameworkCore;

using FilunK.backenddotnet_trial.Models;
using FilunK.backenddotnet_trial.Models.Configure;
using FilunK.backenddotnet_trial.Models.Authentication;
using FilunK.backenddotnet_trial.Utils;
using FilunK.backenddotnet_trial.DataAccess;
using FilunK.backenddotnet_trial.DataAccess.DataModel;


namespace FilunK.backenddotnet_trial.Controllers
{

    /// <summary>
    ///     認証系API
    /// </sumamry>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppSettings _config;
        private readonly PgContext _context;

        private readonly string ProgramId = "Authentication";

        public AuthenticationController(IOptions<AppSettings> configuration, PgContext context)
        {
            this._config = configuration.Value;
            this._context = context;
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
                var tokenSet = this.CreateTokenSet(user);
                response = Ok(tokenSet);
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


        /// <summary>
        ///     リフレッシュトークン更新
        /// </summary>
        /// <remark>
        ///     このAPIでは以下のJSONを<c>PUT</c>メソッドで受け取ります。
        ///     <code>
        ///         {
        ///             refreshToken: string
        ///          }
        ///     </code>
        ///     リフレッシュトークンのバリデーションに成功した場合、更新したJWTを返却します。
        /// </remark>
        ///
        [Route("refresh")]
        [AllowAnonymous]
        [HttpPut]
        public IActionResult Refresh([FromBody] RefreshRequestModel model)
        {
            IActionResult response = Unauthorized();

            var user = this.ReAuthenticate(model.RefreshToken, out var isReAuthenticated);

            if (isReAuthenticated && user != null)
            {
                // JWT生成
                var tokenSet = this.CreateTokenSet(user);
                response = Ok(tokenSet);

            }

            return response;
        }

        /// <summary>
        ///     ユーザ登録
        /// </summary>
        /// <remark>
        ///     このAPIでは以下のJSONを<c>PUT</c>メソッドで受け取ります。
        ///     <code>
        ///         {
        ///             userName: string
        ///             password: string
        ///             mailAddress: string
        ///          }
        ///     </code>
        ///     登録に成功した場合、HTTPコード200とともに、以下のレスポンスを返します。
        ///     これは、2段階認証のためのアクセス用URIです。
        ///     <code>
        ///         {
        ///             confirmationUri: string
        ///             limits: int
        ///          }
        ///     </code>
        ///     ログイン登録に失敗する場合、以下のエラーを返します。
        ///     400: ユーザID・パスワード・メールアドレスの要件を満たしていない。
        ///     403: ユーザIDがすでに登録されている。
        /// </remark>
        ///
        [Route("registration")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult RegistUser([FromBody] RegistRequestModel model)
        {
            IActionResult response = BadRequest();

            // 渡されたモデルのバリデーション
            if (model.ValidateProperties())
            {

                var expireLimit = DateTime.Now.AddMinutes(this._config.Security.ExpireMinutes);
                var comfirmUri = String.Empty;
                //  登録処理
                // IDの重複があるかどうか
                using (this._context)
                {
                    // テーブルのメンテナンス
                    this.MaintainanceUserData(this._context);

                    var registed =
                        (from users in this._context.Users
                         where users.UserId == model.Username
                         select "1").Count();

                    if (registed != 0)
                    {
                        return Forbid();
                    }

                    // SALTとパスワードハッシュの生成
                    var salt = SecurityUtil.GenerateSalt(this._config.Security.ByteSize);
                    var passwordHash = SecurityUtil.GeneratePasswordHash(model.Password, salt, this._config.Security.ByteSize, this._config.Security.Iteration);

                    var saltText = Convert.ToBase64String(salt);
                    var passwordText = Convert.ToBase64String(passwordHash);

                    var rowUserModel = new User()
                    {
                        UserId = model.Username,
                        MailAddress = model.MailAddress,
                        Salt = saltText,
                        Hash = passwordText,
                        Iteration = this._config.Security.Iteration,
                        IsConfirmed = false,
                        CreationId = this.ProgramId,
                        CreationProgram = "RegistUser",
                        UpdateId = this.ProgramId,
                        UpdateProgram = "RegistUser",
                    };

                    this._context.Users.Add(rowUserModel);

                    //  ログイン認証用URIの生成
                    var tempuri = SecurityUtil.GenerateAuthTemporaryString();
                    comfirmUri = "api/v1/authentication/registration/" + tempuri;

                    var rowAccountConfirmModel = new AccountConfirm()
                    {
                        ConfirmUri = tempuri,
                        UserId = rowUserModel.UserId,
                        ExpireLimit = expireLimit,
                        CreationId = this.ProgramId,
                        CreationProgram = "RegistUser",
                        UpdateId = this.ProgramId,
                        UpdateProgram = "RegistUser",
                    };

                    this._context.AccountConfirms.Add(rowAccountConfirmModel);

                    this._context.SaveChanges();
                }

                // レスポンスの生成
                var responseData = new RegistResponse()
                {
                    ConfirmUri = comfirmUri,
                    Limits = expireLimit.ToString()
                };

                response = Ok(responseData);

            }
            else
            {
                response = ValidationProblem(new ValidationProblemDetails()
                {
                    Detail = "ユーザID・パスワード・メールアドレスが要件を満たしていません。"
                });
            }

            return response;
        }
        [Route("registration/{tempUri}")]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ConfirmRegist(string tempUri)
        {
            IActionResult response = BadRequest();

            // 有効なtempUriであるかどうかを確かめる

            using (this._context)
            {
                var queryResult =
                    from confirms in this._context.AccountConfirms
                    join users in this._context.Users
                    on confirms.UserId equals users.UserId
                    where
                        confirms.IsDeleted == false
                        && confirms.ConfirmUri == tempUri
                        && confirms.ExpireLimit >= DateTime.Now
                        && users.IsDeleted == false
                        && users.IsConfirmed == false
                    select new
                    {
                        UserId = users.UserId,
                    };

                // ヒットがなければ404
                if (queryResult.Count() != 1)
                {
                    response = NotFound();
                }

                // データの更新
                var queryData = queryResult.FirstOrDefault();

                var user =
                    (
                        from users in this._context.Users
                        where users.UserId == queryData.UserId
                        select users
                    ).FirstOrDefault();

                user.IsConfirmed = true;
                user.UpdateId = this.ProgramId;
                user.UpdateProgram = "ConfirmRegist";
                user.UpdateDate = DateTime.Now;

                this._context.Users.Update(user);

                var accountConfirm =
                    (from confirms in this._context.AccountConfirms
                     where confirms.ConfirmUri == tempUri
                     select confirms).FirstOrDefault();

                accountConfirm.UpdateId = this.ProgramId;
                accountConfirm.UpdateProgram = "ConfirmRegist";
                accountConfirm.UpdateDate = DateTime.Now;
                accountConfirm.IsDeleted = true;

                this._context.AccountConfirms.Update(accountConfirm);

                this._context.SaveChanges();

                response = Ok();
            }

            return response;
        }

        #region 非HTTPメソッド

        private TokenResponse CreateTokenSet(UserModel model)
        {
            // JWT生成
            var generatedToken = TokenUtil.GenerateToken(
                this._config.Jwt.Issuer,
                this._config.Jwt.Issuer,
                this._config.Jwt.ExpireMinutes,
                model,
                this._config.Jwt.TokenKey, SecurityAlgorithms.HmacSha256
            );
            var refreshToken = TokenUtil.GenerateRefreshToken(
                this._config.Jwt.Issuer,
                this._config.Jwt.Issuer,
                generatedToken,
                this._config.Jwt.RefreshTokenKey,
                this._config.Jwt.ExpireRefreshDays,
                SecurityAlgorithms.HmacSha512
            );

            return new TokenResponse()
            {
                Token = generatedToken,
                Refresh = refreshToken
            };
        }

        /// <summary>
        ///     ユーザ情報を取得する
        /// </summary>
        private UserModel Authenticate(LoginModel login)
        {
            UserModel user = null;

            using (this._context)
            {
                var userData =
                    (
                        from users in this._context.Users
                        where
                            users.IsDeleted == false
                            && users.UserId == login.Username
                            && users.IsConfirmed == true
                        select users
                     ).FirstOrDefault();

                if (userData != null)
                {
                    // 入力されたパスワードとDBのパスワードを検証
                    var salt = Convert.FromBase64String(userData.Salt);
                    var loginHash = SecurityUtil.GeneratePasswordHash(login.Password, salt, this._config.Security.ByteSize, this._config.Security.Iteration);
                    var passwordText = Convert.ToBase64String(loginHash);

                    if (passwordText == userData.Hash)
                    {
                        // UserModelの構築
                        user = new UserModel()
                        {
                            UserName = userData.UserId,
                            MailAddress = userData.MailAddress
                        };
                    }
                }
            }

            return user;
        }

        private UserModel ReAuthenticate(string refreshToken, out bool isValidated)
        {

            UserModel model;
            try
            {
                // refreshTokenのデコード
                model = TokenUtil.DecodeRefreshToken(refreshToken, this._config.Jwt.RefreshTokenKey, this._config.Jwt.TokenKey, this._config.Jwt.Issuer);

                // DBデータとの整合性を図る
                using (this._context)
                {
                    var userData =
                        (
                            from users in this._context.Users
                            where
                                users.IsDeleted == false
                                && users.UserId == model.UserName
                                && users.MailAddress == model.MailAddress
                                && users.IsConfirmed == true
                            select users
                        ).FirstOrDefault();

                    if (userData == null)
                    {
                        isValidated = false;
                        return null;
                    }
                }

            }
            catch
            {
                isValidated = false;
                return null;

            }

            isValidated = true;
            return model;
        }



        private void MaintainanceUserData(PgContext context)
        {
            // 未確認ユーザかつ、一時確認URLが期限切れとなっているデータを検索
            var currentTimestamp = DateTime.Now;
            var queryResult =
                from confirm in context.AccountConfirms
                join users in context.Users
                    on confirm.UserId equals users.UserId
                where
                    confirm.IsDeleted == false
                    && users.IsDeleted == false
                    && users.IsConfirmed == false
                    && confirm.ExpireLimit < currentTimestamp
                select new
                {
                    UserId = users.UserId,
                    TempUri = confirm.ConfirmUri,
                    ExpireLimit = confirm.ExpireLimit,
                    UtcExpireTick = System.TimeZoneInfo.ConvertTimeToUtc(confirm.ExpireLimit).Ticks,
                    NowTick = DateTime.Now.Ticks
                };

            if (queryResult.Count() > 0)
            {
                var targetUsers = 
                (
                    from users in context.Users
                    join queried in queryResult
                        on users.UserId equals queried.UserId
                    select 
                        users
                ).ToArray();

                var targetConfirms =
                (
                    from confirms in context.AccountConfirms
                    join queried in queryResult
                        on confirms.ConfirmUri equals queried.TempUri
                    select
                        confirms
                ).ToArray();

                context.Users.RemoveRange(targetUsers);
                context.AccountConfirms.RemoveRange(targetConfirms);

                context.SaveChanges();
            }

        }
        #endregion
    }
}
