using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
	public interface IUserService
	{
		public Task<ErrorOr<Success>> CreateUserAsync(User user, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> DeleteUserByIdAsync(Guid guid, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> UpdateUser(User user, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> AddRoleAsync(Guid guid, string Role, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> DeleteRoleAsync(Guid guid, string Role, CancellationToken cancellationToken);
		public Task<ErrorOr<List<Role>>> GetRolesListAsync(Guid guid, CancellationToken cancellationToken);
		public Task<ErrorOr<User>> GetUserByIdAsync(Guid guid, CancellationToken cancellationToken);
		public Task<ErrorOr<User>> GetUserByNameAsync(string name, CancellationToken cancellationToken);

	}
}
