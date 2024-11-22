using ErrorOr;
using IB3Api.App;
using IB3Api.App.Interfaces.Repository;
using IB3Api.App.Interfaces.Services;
using IB3Api.Core.Models;
using IB3Api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Services
{
	public class ProfileService : IProfileService
	{
		private readonly ProfileRepository _profileRepository;

		public ProfileService(ProfileRepository profileRepository)
		{
			_profileRepository = profileRepository;
		}

	}
}
