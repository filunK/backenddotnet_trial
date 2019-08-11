using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;

namespace FilunK.backenddotnet_trial.Utils
{
    public static class SecurityUtil
    {
        /// <summary>
        /// SALTを生成する
        /// </summary>
        public static byte[] GenerateSalt(int size)
        {

            var salt = new byte[size];

            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(salt);
            }

            return salt;
        }

        /// <summary>
        /// パスワードのハッシュを求める
        /// </summary>
        public static byte[] GeneratePasswordHash(string password, byte[] salt, int size, int iteration)
        {

            using(var provider = new Rfc2898DeriveBytes(password, salt, iteration))
            {
                return provider.GetBytes(size);
            }

        }

        /// <summary>
        /// 2段階認証のための一時URIを生成する
        /// </summary>
        public static string GenerateAuthTemporaryString()
        {
            const int LENGTH = 40; 
            const string CHARACTERS = "abcdefghijklmnopqrstuvwxyz01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ-_";

            var r = new Random();
            var builder = new StringBuilder(LENGTH);

            for (int i = 0; i < LENGTH; i++)
            {
                var position = r.Next(LENGTH);
                builder.Append(CHARACTERS[position]);
                
            }

            return builder.ToString();
        }
    }
}