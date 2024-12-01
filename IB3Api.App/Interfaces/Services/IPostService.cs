using ErrorOr;
using IB3Api.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.App.Interfaces.Services
{
	public interface IPostService
	{
		public Task<ErrorOr<Success>> UpdatePostTitleAsync(Guid postId, string newTitle, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> UpdatePostTextAsync(Guid postId, string newText, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> UpdatePostImageAsync(Guid postId, string newImage, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> UpdatePostAsync(Post post, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> DeletePostByIdAsync(Guid postId, CancellationToken cancellationToken);
		public Task<ErrorOr<Success>> AddPostAsync(Post post, CancellationToken cancellationToken);
		public Task<ErrorOr<List<Post>>> GetAllPostsAsync(CancellationToken cancellationToken);
		public Task<ErrorOr<Post>> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken);
		public Task<ErrorOr<Guid>> GetCreatorById(Guid postId, CancellationToken cancellationToken);


	}
}
