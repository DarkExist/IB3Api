using ErrorOr;
using IB3Api.App;
using IB3Api.App.Interfaces.Repository;
using IB3Api.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Infrastructure.Repositories
{
	public class ProfileRepository : IProfileRepository
	{
		private readonly ApplicationContext _context;

		public ProfileRepository(ApplicationContext context)
		{
			_context = context;
		}

		public async Task<ErrorOr<Success>> AddAsync(Profile entity, CancellationToken cancellationToken)
		{
			try
			{
				_context.Profiles.Add(entity);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error adding the profile", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrProfile = await GetByIdAsync(id, cancellationToken);
				if (errorOrProfile.IsError)
					return errorOrProfile.Errors;

				Profile profile = errorOrProfile.Value;
				_context.Profiles.Remove(profile);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error deleting the profile", ex.Message);
			}
		}

		public async Task<ErrorOr<List<Profile>>> GetAllAsync(CancellationToken cancellationToken)
		{
			try
			{
				return await _context.Profiles.ToListAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Error.Failure("Error getting all profiles", ex.Message);
			}
		}

		public async Task<ErrorOr<Profile>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				Profile profile = await _context.Profiles.FirstAsync(p => p.Id == id);
				return profile;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error while searching", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> UpdateAsync(Profile updatedProfile, CancellationToken cancellationToken)
		{
			try
			{
				Guid updatedProfileGuid = updatedProfile.Id;
				var errorOrProfile = await GetByIdAsync(updatedProfileGuid, cancellationToken);

				if (errorOrProfile.IsError)
					return Error.Failure("No post to update. Create new");

				_context.Profiles.Update(updatedProfile);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating profile", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> UpdateDescriptionByIdAsync(Guid id, string newDescription, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrProfile = await GetByIdAsync(id, cancellationToken);
				if (errorOrProfile.IsError)
					return Error.Failure("No post to update. Create new");

				Profile newProfile = errorOrProfile.Value;
				newProfile.Description = newDescription;

				var result = await UpdateAsync(newProfile, cancellationToken);
				if (result.IsError)
					return result.Errors;

				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating profile", ex.Message);
			}
		}
	}
}
