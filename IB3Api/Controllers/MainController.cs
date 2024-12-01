using ErrorOr;
using IB3Api.App.Interfaces.Services;
using IB3Api.App.Models.CustomClaim;
using IB3Api.Controllers;
using IB3Api.Core.Models;
using IB3Api.Infrastructure.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace IB3Api.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MainController : ControllerBase
	{
		/*TODO Сделать сигнатуры и расшифровку как у логин контроллеров(принимает шифрованную строку
		 дешифрует и делает с ней все что надо*/

		private readonly ILogger<MainController> _logger;
		private readonly IPostService _postService;
		private readonly IUserService _userService;
		private readonly IRoleService _roleService;
		private readonly IEncryptionService _encryptionService;

		public MainController(ILogger<MainController> logger,
			IPostService postService,
			IUserService userService,
			IRoleService roleService,
			IEncryptionService encryptionService)
		{
			_logger = logger;
			_postService = postService;
			_userService = userService;
			_roleService = roleService;
			_encryptionService = encryptionService;
		}

		[HttpGet("GetAllPosts")]
		public async Task<ActionResult<string>> GetAllPosts()
		{
			BigInteger p, g, A, B, secretKey;
			string encryptedCookie, username;

			try
			{
				string pgABCookie = HttpContext.Request.Headers["Cookie"];
				var pgAB = TryGetpgAGValue(pgABCookie);

				Tuple<BigInteger, BigInteger, BigInteger, BigInteger> key = pgAB;

				secretKey = DataHolder.SecurityKeysHolder[key].Item2;
			}
			catch (Exception ex)
			{
				return StatusCode(399, Error.Failure("Not secured connection"));
			}


			if (HttpContext.Request.Cookies.TryGetValue("AuthCookie", out encryptedCookie))
			{
				try
				{
					var errorOrUsername = GetNameFromCookies(encryptedCookie);
					if (errorOrUsername.IsError)
						return Unauthorized(Error.Failure("No cookies"));
					username = errorOrUsername.Value;
				}
				catch (Exception ex)
				{
					return Unauthorized(Error.Failure("No cookies"));
				}
			}
			else
				return Unauthorized(Error.Failure("No cookies"));



			var errorOrPosts = await _postService.GetAllPostsAsync(HttpContext.RequestAborted);
			if (errorOrPosts.IsError)
				return StatusCode(500, errorOrPosts);

			var errorOrUser = await _userService.GetUserByNameAsync(username, HttpContext.RequestAborted);
			if (errorOrUser.IsError)
				return StatusCode(500, errorOrUser);

			User user = errorOrUser.Value;
			Guid userId = user.Id;

			List<Post> posts = errorOrPosts.Value;
			Dictionary<Guid, List<Post>> responce = new Dictionary<Guid, List<Post>>();

			var errorOrRole = await _roleService.GetRoleByNameAsync("ADMIN", HttpContext.RequestAborted);
			if (errorOrRole.IsError)
				return StatusCode(500);

			Role adminRole = errorOrRole.Value;

			if (user.Roles.Contains(adminRole))
				responce.Add(Guid.Parse("00000000-0000-0000-0000-000000000000"), posts);
			else
				responce.Add(userId, posts);

			string responceSerialized = JsonConvert.SerializeObject(responce);

			string encryptedData = _encryptionService.Encrypt(responceSerialized, secretKey.ToString());

			return Ok(encryptedData);
		}

		[HttpPost("EditPost")]
		public async Task<ActionResult<Success>> EditPost([FromBody] Dictionary<string, string> data)
		{
			BigInteger p, g, A, B, secretKey;
			string encryptedCookie, username, decryptedData;
			EditPostDTO editPostDTO;

			try
			{
				string pgABCookie = HttpContext.Request.Headers["Cookie"];
				var pgAB = TryGetpgAGValue(pgABCookie);

				Tuple<BigInteger, BigInteger, BigInteger, BigInteger> key = pgAB;

				secretKey = DataHolder.SecurityKeysHolder[key].Item2;
			}
			catch (Exception ex)
			{
				return StatusCode(399, Error.Failure("Not secured connection"));
			}


			if (HttpContext.Request.Cookies.TryGetValue("AuthCookie", out encryptedCookie))
			{
				try
				{
					var errorOrUsername = GetNameFromCookies(encryptedCookie);
					if (errorOrUsername.IsError)
						return Unauthorized(Error.Failure("No cookies"));
					username = errorOrUsername.Value;
				}
				catch (Exception ex)
				{
					return Unauthorized(Error.Failure("No cookies"));
				}
			}
			else
				return Unauthorized(Error.Failure("No cookies"));

			try
			{
				decryptedData = _encryptionService.Decrypt(data["EncodedEditPost"], secretKey.ToString());
				editPostDTO = JsonConvert.DeserializeObject<EditPostDTO>(decryptedData);
				if (editPostDTO == null
					|| editPostDTO.Id == null
					|| editPostDTO.Title == null
					|| editPostDTO.Text == null
					|| editPostDTO.Image == null)
				{
					throw new Exception("Bad format");
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}

			Post post = new();
			post.Id = editPostDTO.Id;
			post.Title = editPostDTO.Title;
			post.Text = editPostDTO.Text;
			post.Image = editPostDTO.Image;




			User user = (await _userService.GetUserByNameAsync(username, HttpContext.RequestAborted)).Value;


			var errorOrPost = await _postService.GetPostByIdAsync(post.Id, HttpContext.RequestAborted);
			if (errorOrPost.IsError)
				return BadRequest(errorOrPost);

			var errorOrRole = await _roleService.GetRoleByNameAsync("ADMIN", HttpContext.RequestAborted);
			if (errorOrRole.IsError)
				return StatusCode(500);

			Role adminRole = errorOrRole.Value;

			if (errorOrPost.Value.CreatorId != user.Id && !user.Roles.Contains(adminRole))
				return StatusCode(395, "No permission");


			var errorOrSuccess = await _postService.UpdatePostAsync(post, HttpContext.RequestAborted);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}

		//[HttpPost("editposttext")]
		//public async Task<ActionResult<Success>> EditPostText(Guid postId, string value)
		//{
		//	BigInteger p, g, A, B, secretKey;
		//	string pgAB, encryptedCookie, username;

		//	if (HttpContext.Request.Cookies.TryGetValue("pgAB", out pgAB))
		//	{
		//		var errorOrSecretKey = GetSecretKey(pgAB);
		//		if (errorOrSecretKey.IsError)
		//			return BadRequest("Not secured connection");
		//		secretKey = errorOrSecretKey.Value;
		//	}
		//	else
		//		return BadRequest(Error.Failure("Not secured connection"));

		//	if (HttpContext.Request.Cookies.TryGetValue("AuthCookie", out encryptedCookie))
		//	{
		//		var errorOrUsername = GetNameFromCookies(encryptedCookie);
		//		if (errorOrUsername.IsError)
		//			return Unauthorized(Error.Failure("No cookies"));

		//		username = errorOrUsername.Value;
		//	}
		//	else
		//		return Unauthorized(Error.Failure("No cookies"));

		//	User user = (await _userService.GetUserByNameAsync(username, HttpContext.RequestAborted)).Value;


		//	var errorOrPost = await _postService.GetPostByIdAsync(postId, HttpContext.RequestAborted);
		//	if (errorOrPost.IsError)
		//		return BadRequest(errorOrPost);

		//	var errorOrRole = await _roleService.GetRoleByNameAsync("ADMIN", HttpContext.RequestAborted);
		//	if (errorOrRole.IsError)
		//		return StatusCode(500);

		//	Role adminRole = errorOrRole.Value;

		//	if (errorOrPost.Value.CreatorId != user.Id && !user.Roles.Contains(adminRole))
		//		return BadRequest("No permission");

		//	var errorOrSuccess = await _postService.UpdatePostTextAsync(postId, value, CancellationToken.None);
		//	if (errorOrSuccess.IsError)
		//		return BadRequest(errorOrSuccess);

		//	return Ok();
		//}

		//[HttpPost("editpostimage")]
		//public async Task<ActionResult<Success>> EditPostImage(Guid postId, string value)
		//{
		//	BigInteger p, g, A, B, secretKey;
		//	string pgAB, encryptedCookie, username;

		//	if (HttpContext.Request.Cookies.TryGetValue("pgAB", out pgAB))
		//	{
		//		var errorOrSecretKey = GetSecretKey(pgAB);
		//		if (errorOrSecretKey.IsError)
		//			return BadRequest("Not secured connection");
		//		secretKey = errorOrSecretKey.Value;
		//	}
		//	else
		//		return BadRequest(Error.Failure("Not secured connection"));

		//	if (HttpContext.Request.Cookies.TryGetValue("AuthCookie", out encryptedCookie))
		//	{
		//		var errorOrUsername = GetNameFromCookies(encryptedCookie);
		//		if (errorOrUsername.IsError)
		//			return Unauthorized(Error.Failure("No cookies"));

		//		username = errorOrUsername.Value;
		//	}
		//	else
		//		return Unauthorized(Error.Failure("No cookies"));

		//	User user = (await _userService.GetUserByNameAsync(username, HttpContext.RequestAborted)).Value;


		//	var errorOrPost = await _postService.GetPostByIdAsync(postId, HttpContext.RequestAborted);
		//	if (errorOrPost.IsError)
		//		return BadRequest(errorOrPost);

		//	var errorOrRole = await _roleService.GetRoleByNameAsync("ADMIN", HttpContext.RequestAborted);
		//	if (errorOrRole.IsError)
		//		return StatusCode(500);

		//	Role adminRole = errorOrRole.Value;

		//	if (errorOrPost.Value.CreatorId != user.Id && !user.Roles.Contains(adminRole))
		//		return BadRequest("No permission");

		//	var errorOrSuccess = await _postService.UpdatePostImageAsync(postId, value, CancellationToken.None);
		//	if (errorOrSuccess.IsError)
		//		return BadRequest(errorOrSuccess);

		//	return Ok();
		//}


		[HttpPost("AddPost")]
		public async Task<ActionResult<Success>> AddPost([FromBody] Dictionary<string, string> data)
		{
			//string postTitle, string postText, string postImage
			BigInteger p, g, A, B, secretKey;
			string encryptedCookie, username, decryptedData;
			PostDTO postDTO;

			try
			{
				string pgABCookie = HttpContext.Request.Headers["Cookie"];
				var pgAB = TryGetpgAGValue(pgABCookie);

				Tuple<BigInteger, BigInteger, BigInteger, BigInteger> key = pgAB;

				secretKey = DataHolder.SecurityKeysHolder[key].Item2;
			}
			catch (Exception ex)
			{
				return StatusCode(399, Error.Failure("Not secured connection"));
			}


			if (HttpContext.Request.Cookies.TryGetValue("AuthCookie", out encryptedCookie))
			{
				try
				{
					var errorOrUsername = GetNameFromCookies(encryptedCookie);
					if (errorOrUsername.IsError)
						return Unauthorized(Error.Failure("No cookies"));
					username = errorOrUsername.Value;
				}
				catch (Exception ex)
				{
					return Unauthorized(Error.Failure("No cookies"));
				}
			}
			else
				return Unauthorized(Error.Failure("No cookies"));

			try
			{
				decryptedData = _encryptionService.Decrypt(data["EncodedPost"], secretKey.ToString());
				postDTO = JsonConvert.DeserializeObject<PostDTO>(decryptedData);
				if (postDTO == null
					|| postDTO.Title == null
					|| postDTO.Text == null
					|| postDTO.Image == null)
				{
					throw new Exception("Bad format");
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}

			User user = (await _userService.GetUserByNameAsync(username, HttpContext.RequestAborted)).Value;

			Post post = new();
			post.Title = postDTO.Title;
			post.Text = postDTO.Text;
			post.Image = postDTO.Image;
			post.CreatorId = user.Id;
			post.CreatedAt = DateTime.UtcNow;

			var errorOrSuccess = await _postService.AddPostAsync(post, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}

		[HttpPost("DeletePost")]
		public async Task<ActionResult<Success>> DeletePost([FromBody] Dictionary<string, string> data)
		{
			BigInteger p, g, A, B, secretKey;
			string encryptedCookie, username, decryptedData;
			DeletePostDTO deletePostDTO;

			try
			{
				string pgABCookie = HttpContext.Request.Headers["Cookie"];
				var pgAB = TryGetpgAGValue(pgABCookie);

				Tuple<BigInteger, BigInteger, BigInteger, BigInteger> key = pgAB;

				secretKey = DataHolder.SecurityKeysHolder[key].Item2;
			}
			catch (Exception ex)
			{
				return StatusCode(399, Error.Failure("Not secured connection"));
			}


			if (HttpContext.Request.Cookies.TryGetValue("AuthCookie", out encryptedCookie))
			{
				try
				{
					var errorOrUsername = GetNameFromCookies(encryptedCookie);
					if (errorOrUsername.IsError)
						return Unauthorized(Error.Failure("No cookies"));
					username = errorOrUsername.Value;
				}
				catch (Exception ex)
				{
					return Unauthorized(Error.Failure("No cookies"));
				}
			}
			else
				return Unauthorized(Error.Failure("No cookies"));

			try
			{
				decryptedData = _encryptionService.Decrypt(data["EncodedPostId"], secretKey.ToString());
				deletePostDTO = JsonConvert.DeserializeObject<DeletePostDTO>(decryptedData);
				if (deletePostDTO == null
					|| deletePostDTO.Id == null)
				{
					throw new Exception("Bad format");
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}

			Guid postId = deletePostDTO.Id;

			User user = (await _userService.GetUserByNameAsync(username, HttpContext.RequestAborted)).Value;

			var errorOrPost = await _postService.GetPostByIdAsync(postId, HttpContext.RequestAborted);
			if (errorOrPost.IsError)
				return BadRequest(errorOrPost);

			var errorOrRole = await _roleService.GetRoleByNameAsync("ADMIN", HttpContext.RequestAborted);
			if (errorOrRole.IsError)
				return StatusCode(500);

			Role adminRole = errorOrRole.Value;

			if (errorOrPost.Value.CreatorId != user.Id && !user.Roles.Contains(adminRole))
				return BadRequest("No permission");

			var errorOrSuccess = await _postService.DeletePostByIdAsync(postId, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return BadRequest(errorOrSuccess);

			return Ok();
		}

		private ErrorOr<string> GetNameFromCookies(string encryptedStringClaims)
		{
			string decryptedStringClaims = _encryptionService.Decrypt(encryptedStringClaims);
			List<CustomClaim> claims = JsonConvert.DeserializeObject<List<CustomClaim>>(decryptedStringClaims);
			string username = null;
			foreach (var claim in claims)
			{
				if (claim.Name == "username")
					username = claim.Value;
				if (claim.Name == "exp")
				{
					DateTime parsedDate = DateTime.ParseExact(
						claim.Value,                      // Строка для парсинга
						"yyyy-MM-dd HH:mm:ss'Z'",        // Формат строки
						CultureInfo.InvariantCulture,    // Культура для парсинга
						DateTimeStyles.AssumeUniversal   // Указывает, что время в UTC
					);
					if (DateTime.UtcNow >= parsedDate)
					{
						return Error.Failure("Cookies expired");
					}
				}
			}
			if (username != null)
				return username;

			return Error.Failure("Critical error while checking cookie");
		}
		

		private Tuple<BigInteger, BigInteger, BigInteger, BigInteger> TryGetpgAGValue(string input)
		{
			string pattern = @"pgAB=\{\""p\"":""(\d+)"",""g\"":""(\d+)"",""A\"":""(\d+)"",""B\"":""(\d+)\""\}";
			Match match = Regex.Match(input, pattern);

			if (match.Success)
			{
				string p = match.Groups[1].Value;
				string g = match.Groups[2].Value;
				string A = match.Groups[3].Value;
				string B = match.Groups[4].Value;

				return Tuple.Create(BigInteger.Parse(p), BigInteger.Parse(g),
					BigInteger.Parse(A), BigInteger.Parse(B));
			}
			throw new Exception("Wrong pgAB Value");
		}

		private Tuple<BigInteger, BigInteger, BigInteger, BigInteger> TryGetAuthValue(string input)
		{
			string pattern = @"pgAB=\{\""p\"":""(\d+)"",""g\"":""(\d+)"",""A\"":""(\d+)"",""B\"":""(\d+)\""\}";
			Match match = Regex.Match(input, pattern);

			if (match.Success)
			{
				string p = match.Groups[1].Value;
				string g = match.Groups[2].Value;
				string A = match.Groups[3].Value;
				string B = match.Groups[4].Value;

				return Tuple.Create(BigInteger.Parse(p), BigInteger.Parse(g),
					BigInteger.Parse(A), BigInteger.Parse(B));
			}
			throw new Exception("Wrong pgAB Value");
		}
	}
}

