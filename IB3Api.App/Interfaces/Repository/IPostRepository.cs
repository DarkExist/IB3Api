using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Repository
{
	public interface IPostRepository : IRepository<Post>
	{
		Task<ErrorOr<Success>> UpdateTitleByIdAsync(Guid id, string newTitle, CancellationToken cancellationToken);
		Task<ErrorOr<Success>> UpdateTextByIdAsync(Guid id, string newText, CancellationToken cancellationToken);
		Task<ErrorOr<Success>> UpdateImageByIdAsync(Guid id, string newImage, CancellationToken cancellationToken);
	}
}
