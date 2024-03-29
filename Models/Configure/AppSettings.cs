using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilunK.backenddotnet_trial.Models.Configure
{
    public class AppSettings
    {

        public Logging Logging { get; set; }
        public Jwt Jwt { get; set; }

        public Security Security { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }

        public string AllowedHosts { get; set; }
    }

    #region AppSettingの子クラス
    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class Jwt
    {
        public string TokenKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public string Issuer { get; set; }

        public double ExpireMinutes { get; set; }
        public double ExpireRefreshDays { get; set; }
    }

    public class ConnectionStrings
    {
        public string Postgres { get; set; }
    }

    public class Security
    {
        public int ByteSize { get; set; }
        public int Iteration { get; set; }
        public double ExpireMinutes { get; set; }
    }

    #endregion AppSettingの子クラス

}