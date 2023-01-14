using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal class ProtectionService : IProtectionService
    {
        public string Decrypt(string encryptedValue)
        {
            if(encryptedValue == null)
            {
                throw new ArgumentNullException(nameof(encryptedValue));
            }

            var encryptedData = Convert.FromBase64String(encryptedValue);
            var data = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(data);
        }

        public string Encrypt(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var data = Encoding.UTF8.GetBytes(value);
            var encryptedData = ProtectedData.Protect(data,null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }
    }
}
