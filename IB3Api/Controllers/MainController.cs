using ErrorOr;
using IB3Api.App.Interfaces.Services;
using IB3Api.Controllers;
using IB3Api.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Numerics;
using System.Security.Claims;

namespace IB3Api.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MainController : ControllerBase
	{
		private readonly ILogger<MainController> _logger;
		private readonly IPostService _postService;

		public MainController(ILogger<MainController> logger,
			IPostService postService)
		{
			_logger = logger;
			_postService = postService;
		}

		[HttpGet]
		public async Task<ActionResult<List<Post>>> GetAllPosts()
		{
			// check authorize

			var errorOrPosts = await _postService.GetAllPostsAsync(CancellationToken.None);

			if (errorOrPosts.IsError)
				return BadRequest(errorOrPosts);

			List<Post> posts = errorOrPosts.Value;

			return Ok(posts);
		}

		[HttpPost]
		public async Task<ActionResult<Success>> EditPostTitle(Guid postId, string newTitle)
		{
			// check authorize

			// check ownership of post(or is admin)

			var errorOrSuccess = await _postService.UpdatePostTitleAsync(postId, newTitle, CancellationToken.None);
			

			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<Success>> EditPostText(Guid postId, string newText)
		{
			// check authorize

			// check ownership of post(or is admin)

			var errorOrSuccess = await _postService.UpdatePostTextAsync(postId, newText, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<Success>> EditPostImage(Guid postId, string newImage)
		{
			// check authorize

			// check ownership of post(or is admin)

			var errorOrSuccess = await _postService.UpdatePostImageAsync(postId, newImage, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}


		[HttpPost]
		public async Task<ActionResult<Success>> AddPost(string postTitle, string postText, string postImage)
		{
			// check authorize

			Post post = new();
			post.Title = postTitle;
			post.Text = postText;
			post.Image = postImage;

			var errorOrSuccess = await _postService.AddPostAsync(post, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<Success>> DeletePost(Guid postId)
		{
			// check authorize

			// check ownership of post(or is admin)

			var errorOrSuccess = await _postService.DeletePostByIdAsync(postId, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}
	}
}

