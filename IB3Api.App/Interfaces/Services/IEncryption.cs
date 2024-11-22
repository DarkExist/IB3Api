using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
    public interface IEncryption
    {
        public string Encrypt(string data);
        public string Decrypt(string encryptedData);
    }
}
