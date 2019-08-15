using System;
using System.Text.RegularExpressions;

namespace FilunK.backenddotnet_trial.Models.Authentication
{
    public class RegistRequestModel : LoginModel, IValidatable
    {
        /// <summary>
        ///     メールアドレス
        /// </summary>
        public string MailAddress { get; set; }
        

        public override bool ValidateProperties()
        {
            var baseResult = base.ValidateProperties();

            if (baseResult)
            {
                var mailAddressRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$" ;

                return !String.IsNullOrEmpty(this.MailAddress) && Regex.IsMatch(this.MailAddress, mailAddressRegex,RegexOptions.IgnoreCase);
            }

            return false;
        }

    }
}
