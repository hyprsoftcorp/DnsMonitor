using Microsoft.AspNetCore.DataProtection;
using System;
using System.Text;

namespace Hyprsoft.Dns.Monitor
{
    public class DataProtector
    {
        #region Fields

        private static readonly string _namespace;

        #endregion

        #region Constructors

        static DataProtector() => _namespace = typeof(DataProtector).Namespace;

        #endregion

        #region Methods

        public static string EncryptString(string plainText)
        {
            if (String.IsNullOrWhiteSpace(plainText))
                return plainText;

            var dp = DataProtectionProvider.Create(_namespace);
            var protector = dp.CreateProtector(_namespace);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(protector.Protect(plainText)));
        }

        public static string DecryptString(string secret)
        {
            if (String.IsNullOrWhiteSpace(secret))
                return secret;

            var dp = DataProtectionProvider.Create(_namespace);
            var protector = dp.CreateProtector(_namespace);
            return protector.Unprotect(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(secret)));
        }

        #endregion
    }
}
