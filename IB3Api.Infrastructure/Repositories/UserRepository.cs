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
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationContext _context;

		public UserRepository(ApplicationContext context)
		{
			_context = context;
		}

		public async Task<ErrorOr<Success>> AddAsync(User entity, CancellationToken cancellationToken)
		{
			try
			{
				_context.Users.Add(entity);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error adding the user", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrUser = await GetByIdAsync(id, cancellationToken);
				if (errorOrUser.IsError)
					return errorOrUser.Errors;

				User user = errorOrUser.Value;
				_context.Users.Remove(user);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error deleting the user", ex.Message);
			}
		}

		
		public async Task<ErrorOr<List<User>>> GetAllAsync(CancellationToken cancellationToken)
		{
			try
			{
				return await _context.Users.Include(u => u.Roles).ToListAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Error.Failure("Error getting all users", ex.Message);
			}
		}

		public async Task<ErrorOr<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				User user = await _context.Users.Include(u => u.Roles).FirstAsync(p => p.Id == id);
				return user;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error while searching", ex.Message);
			}
		}

		public async Task<ErrorOr<User>> GetByNameAsync(string name, CancellationToken cancellationToken)
		{
			try
			{
				User user = await _context.Users.Include(u => u.Roles).FirstAsync(p => p.Name == name);
				return user;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error while searching", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> UpdateAsync(User updatedUser, CancellationToken cancellationToken)
		{
			try
			{
				Guid updatedUserGuid = updatedUser.Id;
				var errorOrUser = await GetByIdAsync(updatedUserGuid, cancellationToken);

				if (errorOrUser.IsError)
					return Error.Failure("No user to update. You need to create new");

				_context.Users.Update(updatedUser);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating user", ex.Message);
			}
		}
	}
}
