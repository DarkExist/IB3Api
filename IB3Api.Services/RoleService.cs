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
	public class RoleService : IRoleService
	{
		private readonly IRoleRepository _roleRepository;

		public RoleService(IRoleRepository roleRepository)
		{
			_roleRepository = roleRepository;
		}

		public async Task<ErrorOr<Success>> AddRoleAsync(Role role, CancellationToken cancellationToken)
		{
			return await _roleRepository.AddAsync(role, cancellationToken);
		}

		public async Task<ErrorOr<Success>> DeleteRoleByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			return await _roleRepository.DeleteByIdAsync(id, cancellationToken);
		}

		public async Task<ErrorOr<List<Role>>> GetAllRolesAsync(CancellationToken cancellationToken)
		{
			return await _roleRepository.GetAllAsync(cancellationToken);
		}

		public async Task<ErrorOr<Role>> GetRoleByIdAsync(Guid guid, CancellationToken cancellationToken)
		{
			return await _roleRepository.GetByIdAsync(guid, cancellationToken);	
		}

		public async Task<ErrorOr<Role>> GetRoleByNameAsync(string name, CancellationToken cancellationToken)
		{
			return await _roleRepository.GetRoleByNameAsync(name, cancellationToken);
		}
	}
}
