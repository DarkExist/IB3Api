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
		private readonly IProfileRepository _profileRepository;

		public ProfileService(IProfileRepository profileRepository)
		{
			_profileRepository = profileRepository;
		}

		public async Task<ErrorOr<Success>> AddProfileDescriptionAsync(Guid profileId, CancellationToken cancellationToken)
		{
			Profile profile = new();
			profile.Id = profileId;
			profile.Description = "";
			return await _profileRepository.AddAsync(profile, cancellationToken);
		}

		public async Task<ErrorOr<Profile>> GetProfileById(Guid profileId, CancellationToken cancellationToken)
		{
			return await _profileRepository.GetByIdAsync(profileId, cancellationToken);
		}

		public async Task<ErrorOr<Success>> UpdateDescriptionAsync(Guid profileId, string description, CancellationToken cancellationToken)
		{
			return await _profileRepository.UpdateDescriptionByIdAsync(profileId, description, cancellationToken);
		}

	}
}
