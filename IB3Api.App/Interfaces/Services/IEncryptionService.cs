using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
    public interface IEncryptionService
    {
        public string Encrypt(string data, string? key = null);
        public string Decrypt(string encryptedData, string? key = null);
    }
}
