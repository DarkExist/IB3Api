using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorOr;

namespace IB3Api.App.Interfaces.Repository
{
	public interface IRepository<T>
	{
		Task<ErrorOr<List<T>>> GetAllAsync(CancellationToken cancellationToken);
		Task<ErrorOr<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
		Task<ErrorOr<Success>> AddAsync(T entity, CancellationToken cancellationToken);
		Task<ErrorOr<Success>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
		Task<ErrorOr<Success>> UpdateAsync(T entity, CancellationToken cancellationToken);

	}
}
