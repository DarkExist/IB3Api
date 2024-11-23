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
	public class RoleRepository : IRoleRepository
	{
		private readonly ApplicationContext _context;

		public RoleRepository(ApplicationContext context)
		{
			_context = context;
		}

		public async Task<ErrorOr<Success>> AddAsync(Role entity, CancellationToken cancellationToken)
		{
			try
			{
				_context.Roles.Add(entity);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error adding the role", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrRole = await GetByIdAsync(id, cancellationToken);
				if (errorOrRole.IsError)
					return errorOrRole.Errors;

				Role role = errorOrRole.Value;
				_context.Roles.Remove(role);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error deleting the role", ex.Message);
			}
		}

		public async Task<ErrorOr<List<Role>>> GetAllAsync(CancellationToken cancellationToken)
		{
			try
			{
				return await _context.Roles.ToListAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Error.Failure("Error getting all roles", ex.Message);
			}
		}

		public async Task<ErrorOr<Role>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				Role role = await _context.Roles.FirstAsync(p => p.Id == id);
				return role;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error while searching", ex.Message);
			}
		}

		public async Task<ErrorOr<Role>> GetRoleByNameAsync(string name, CancellationToken cancellationToken)
		{
			try
			{
				Role? roleOrNull =  await _context.Roles.FirstOrDefaultAsync((r => r.Name == name), cancellationToken);
				if (roleOrNull == null)
					return Error.Failure("No role with such name");

				return roleOrNull!;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error while getting", ex.Message);
			}

		}

		public async Task<ErrorOr<Success>> UpdateAsync(Role updatedRole, CancellationToken cancellationToken)
		{
			try
			{
				Guid updatedRoleGuid = updatedRole.Id;
				var errorOrRole = await GetByIdAsync(updatedRoleGuid, cancellationToken);

				if (errorOrRole.IsError)
					return Error.Failure("No role to update. Create new");

				_context.Roles.Update(updatedRole);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating profile", ex.Message);
			}
		}
	}
}
