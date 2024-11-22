using ErrorOr;
using IB3Api.App;
using IB3Api.App.Interfaces.Repository;
using IB3Api.App.Interfaces.Services;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Services
{
	public class UserService : IUserService
	{
		private readonly UserService _userRepository;

		public UserService(UserService userRepository)
		{
			_userRepository = userRepository;
		}
		
	}
}
