using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Repository
{
	public interface IUserRepository : IRepository<User>
	{
		public Task<ErrorOr<Success>> AddRoleAsync(Guid id, string role, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> DeleteRoleAsync(Guid id, string role, CancellationToken cancellationToken);
	}
}
