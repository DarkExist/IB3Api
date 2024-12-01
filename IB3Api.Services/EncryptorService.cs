using IB3Api.App.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Services
{
    public class EncryptorService : IEncryptionService
	{
		private readonly IConfiguration _config;
		private readonly byte[] _key;
		private SHA256 _sha256 = SHA256.Create();

		public EncryptorService(IConfiguration config) 
		{
			_config = config;
			_key = Encoding.UTF8.GetBytes(_config["SecretKey"]!);
		}

		 
		public string Decrypt(string encryptedData, string? key = null)
		{
			byte[] decryptKey;
			if (key != null)
				decryptKey = _sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
			else
				decryptKey = _key;

			using var aes = Aes.Create();
			aes.Key = decryptKey;
			aes.Mode = CipherMode.ECB;
			aes.Padding = PaddingMode.PKCS7;

			using var decryptor = aes.CreateDecryptor();
			byte[] ciphertext = Convert.FromBase64String(encryptedData);
			byte[] plaintext = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);

			string result = Encoding.UTF8.GetString(plaintext);
			return result;
		}

		public string Encrypt(string data, string? key = null)
		{
			byte[] decryptKey;
			if (key != null)
			{
				decryptKey = _sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
			}
			else
				decryptKey = _key;

			using var aes = Aes.Create();
			aes.Key = decryptKey;
			aes.Mode = CipherMode.ECB;
			aes.Padding = PaddingMode.PKCS7;

			using var encryptor = aes.CreateEncryptor();
			byte[] plaintext = Encoding.UTF8.GetBytes(data);
			byte[] ciphertext = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);

			return Convert.ToBase64String(ciphertext);
		}
	}
}
