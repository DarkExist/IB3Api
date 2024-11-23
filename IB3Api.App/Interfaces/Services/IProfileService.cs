using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
	public interface IProfileService
	{
		public Task<ErrorOr<Success>> UpdateDescriptionAsync(Guid profileId, string description, CancellationToken cancellationToken);
		public Task<ErrorOr<Profile>> GetProfileById(Guid profileId, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> AddProfileDescriptionAsync(Guid profileId, CancellationToken cancellationToken);
	}
}
