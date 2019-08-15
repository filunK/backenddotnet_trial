using System;
using System.Text.RegularExpressions;

namespace FilunK.backenddotnet_trial.Models.Authentication
{
    /// <summary>
    ///     ログインJSONのモデルクラス
    /// </summary>
    public class LoginModel: IValidatable
    {
        /// <summary>
        ///     ユーザ名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     パスワード
        /// </summary>
        public string Password { get; set; }

        public virtual bool ValidateProperties()
        {
            // Usernameは8-100文字の英数字
            var usernameRegex = @"^[a-zA-Z\d]{8,100}$";
            // Passwordは8-100文字の英数字、小文字・大文字・数字が1つ以上含まれていること。
            var passwordRegex = @"^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?\d)[a-zA-Z\d]{8,100}$";

            if (String.IsNullOrEmpty(this.Username) || String.IsNullOrEmpty(this.Password))
            {
                return false;
            }

            return Regex.IsMatch(this.Username, usernameRegex) && Regex.IsMatch(this.Password, passwordRegex);
        }
    }
}
