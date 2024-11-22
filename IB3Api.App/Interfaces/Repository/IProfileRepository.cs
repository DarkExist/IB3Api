using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Repository
{
	public interface IProfileRepository : IRepository<Profile>
	{
		Task<ErrorOr<Success>> UpdateDescriptionByIdAsync(Guid id, string newDescription, CancellationToken cancellationToken);
	}
}
