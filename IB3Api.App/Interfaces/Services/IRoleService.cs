using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
	public interface IRoleService
	{
		public Task<ErrorOr<Role>> GetRoleByNameAsync(string name, CancellationToken cancellationToken);
		public Task<ErrorOr<Role>> GetRoleByIdAsync(Guid guid, CancellationToken cancellationToken);
		public Task<ErrorOr<List<Role>>> GetAllRolesAsync(CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> AddRoleAsync(Role role, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> DeleteRoleByIdAsync(Guid id, CancellationToken cancellationToken);
	}
}
