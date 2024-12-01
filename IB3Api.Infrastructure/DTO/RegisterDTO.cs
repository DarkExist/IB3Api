using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Infrastructure.DTO
{
	public class RegisterDTO
	{
		public string Username { get; set; }
		public string Password { get; set; }

		public RegisterDTO(string username, string password)
		{
			Username = username;
			Password = password;
		}
	}
}
