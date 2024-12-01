using ErrorOr;
using IB3Api.App;
using IB3Api.App.Interfaces.Repository;
using IB3Api.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IB3Api.Infrastructure.Repositories
{
	public class PostRepository : IPostRepository
	{
		private readonly ApplicationContext _context;

		public PostRepository(ApplicationContext context)
		{
			_context = context;
		}



		public async Task<ErrorOr<Success>> AddAsync(Post entity, CancellationToken cancellationToken)
		{
			try
			{
				_context.Posts.Add(entity);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error adding the post", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrPost = await GetByIdAsync(id, cancellationToken);
				if (errorOrPost.IsError)
					return errorOrPost.Errors;

				Post post = errorOrPost.Value;
				_context.Posts.Remove(post);
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error deleting the post", ex.Message);
			}
		}

		public async Task<ErrorOr<List<Post>>> GetAllAsync(CancellationToken cancellationToken)
		{
			try
			{
				return await _context.Posts.ToListAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Error.Failure("Error getting all post", ex.Message);
			}
		}

		public async Task<ErrorOr<Post>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			try
			{
				Post post = await _context.Posts.FirstAsync(p => p.Id == id);
				return post;
			}
			catch (Exception ex) 
			{
				return Error.Failure("Error while searching", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> UpdateAsync(Post updatedPost, CancellationToken cancellationToken)
		{
			try
			{
				Guid updatedPostGuid = updatedPost.Id;
				var errorOrPost = await GetByIdAsync(updatedPostGuid, cancellationToken);

				if (errorOrPost.IsError)
					return Error.Failure("No post to update. Create new");

				Post existingPost = errorOrPost.Value;
				existingPost.Title = updatedPost.Title;
				existingPost.Text = updatedPost.Text;
				existingPost.Image = updatedPost.Image;
				
				await _context.SaveChangesAsync(cancellationToken);
				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating post", ex.Message);
			}

		}

		public async Task<ErrorOr<Success>> UpdateImageByIdAsync(Guid id, string newImage, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrPost = await GetByIdAsync(id, cancellationToken);
				if (errorOrPost.IsError)
					return Error.Failure("No post to update. Create new");

				Post newPost = errorOrPost.Value;
				newPost.Image = newImage;

				var result = await UpdateAsync(newPost, cancellationToken);
				if (result.IsError)
					return result.Errors;

				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating post", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> UpdateTextByIdAsync(Guid id, string newText, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrPost = await GetByIdAsync(id, cancellationToken);
				if (errorOrPost.IsError)
					return Error.Failure("No post to update. Create new");
					
				Post newPost = errorOrPost.Value;
				newPost.Text = newText;

				var result = await UpdateAsync(newPost, cancellationToken);
				
				if (result.IsError)
					return result.Errors;

				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating post", ex.Message);
			}
		}

		public async Task<ErrorOr<Success>> UpdateTitleByIdAsync(Guid id, string newTitle, CancellationToken cancellationToken)
		{
			try
			{
				var errorOrPost = await GetByIdAsync(id, cancellationToken);
				if (errorOrPost.IsError)
					return Error.Failure("No post to update. Create new");

				Post newPost = errorOrPost.Value;
				newPost.Title = newTitle;

				var result = await UpdateAsync(newPost, cancellationToken);

				if (result.IsError)
					return result.Errors;

				return Result.Success;
			}
			catch (Exception ex)
			{
				return Error.Failure("Error updating post", ex.Message);
			}
		}
	}
}
