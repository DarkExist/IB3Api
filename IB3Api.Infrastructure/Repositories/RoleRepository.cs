using ErrorOr;
using IB3Api.App;
using IB3Api.App.Interfaces.Repository;
using IB3Api.Core.Models;
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

		public Task<ErrorOr<Success>> AddAsync(Role entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ErrorOr<Success>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ErrorOr<List<Role>>> GetAllAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ErrorOr<Role>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ErrorOr<Guid>> GetIdByNameAsync(string Name, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<ErrorOr<Success>> UpdateAsync(Role entity, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
