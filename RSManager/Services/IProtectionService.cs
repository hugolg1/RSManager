using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal interface IProtectionService
    {
        string Encrypt(string value);
        string Decrypt(string encryptedValue);
    }
}
