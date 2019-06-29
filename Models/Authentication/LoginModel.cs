namespace FilunK.backenddotnet_trial.Models.Authentication
{
    /// <summary>
    ///     ログインJSONのモデルクラス
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        ///     ユーザ名
        /// </summary>
        public string Username {get; set;}
        
        /// <summary>
        ///     パスワード
        /// </summary>
        public string Password {get; set;}
    }
}