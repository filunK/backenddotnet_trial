
using FilunK.backenddotnet_trial.Models;
using FilunK.backenddotnet_trial.Models.Authentication;

namespace FilunK.backenddotnet_trial.Procedures.V1
{
    public class SecurityProcedure
    {

        private LoginModel LoginModel{get;set;}
        
        public SecurityProcedure(LoginModel loginModel)
        {
            this.LoginModel = loginModel;
        }

        public bool Validate(LoginModel login)
        {
            // loginのユーザ名からT_USERを取得
            // T_USERのsalt,iterationを利用してloginのパスワードをハッシュ化
            // loginパスワードのハッシュとT_USERのハッシュを比較

            return false;
        }

        // public string Regist(LoginModel login)
        // {

        // }
    }
}