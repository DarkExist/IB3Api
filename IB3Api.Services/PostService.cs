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
	public class PostService : IPostService
	{
		private readonly PostRepository _postRepository;

		public PostService(PostRepository postRepositor)
		{
			_postRepository = postRepositor;
		}

		public async Task<ErrorOr<Success>> AddPostAsync(Post post, CancellationToken cancellationToken)
		{
			return await _postRepository.AddAsync(post, cancellationToken);
		}

		public async Task<ErrorOr<Success>> UpdatePostImageAsync(Guid postId, string newImage, CancellationToken cancellationToken)
		{
			return await _postRepository.UpdateImageByIdAsync(postId, newImage, cancellationToken);
		}

		public async Task<ErrorOr<Success>> UpdatePostTextAsync(Guid postId, string newText, CancellationToken cancellationToken)
		{
			return await _postRepository.UpdateTextByIdAsync(postId, newText, cancellationToken);
		}

		public async Task<ErrorOr<Success>> UpdatePostTitleAsync(Guid postId, string newTitle, CancellationToken cancellationToken)
		{
			return await _postRepository.UpdateTitleByIdAsync(postId, newTitle, cancellationToken);
		}

		public async Task<ErrorOr<Success>> DeletePostByIdAsync(Guid postId, CancellationToken cancellationToken)
		{
			return await _postRepository.DeleteByIdAsync(postId, cancellationToken);
		}



		public async Task<ErrorOr<List<Post>>> GetAllPostsAsync(CancellationToken cancellationToken)
		{
			return await _postRepository.GetAllAsync(cancellationToken);
		}

		public async Task<ErrorOr<Post>> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken)
		{
			return await _postRepository.GetByIdAsync(postId, cancellationToken);
		}

		public async Task<ErrorOr<Guid>> GetCreatorById(Guid postId, CancellationToken cancellationToken)
		{
			var errorOrPost = await GetPostByIdAsync(postId, cancellationToken);

			if (errorOrPost.IsError)
				return Error.Failure("No post with such id");

			Post post = errorOrPost.Value;
			return post.CreatorId;
		}
	}
}
