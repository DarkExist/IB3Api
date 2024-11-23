using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using IB3Api.App.Interfaces.Services







using IB3Api.Api;
using System.Net;
using IB3Api.Core.Models;
using System.Security.Claims;
using Newtonsoft.Json;

namespace IB3Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly ILogger<LoginController> _logger;
		private readonly IUserService _userService;
		private readonly IEncryptionService _encryptionService;

		public LoginController(ILogger<LoginController> logger,
			IUserService userService,
			IEncryptionService encryptionService)
		{
			_logger = logger;
			_userService = userService;
			_encryptionService = encryptionService;
		}

		[HttpPost(Name = "Login")]
		public async Task<ActionResult> PostLogin(string? Auth)
		{
			BigInteger p, g, A, B;
			string pgAB, username, password;
			if (HttpContext.Request.Cookies.TryGetValue("pgAB", out pgAB))
			{
				string asd = "";////
			}

			username = "123";
			password = "123";

			var errorOrUser = await _userService.GetUserByNameAsync(username, CancellationToken.None);

			if (errorOrUser.IsError)
				return BadRequest("No such user");

			User user = errorOrUser.Value;
			if (password != user.Password)
				return BadRequest("Wrong username or password");

			var claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Expiration, 
					DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))
			};

			string jsonClaims = JsonConvert.SerializeObject(claims);
			string EncryptedClaims = _encryptionService.Encrypt(jsonClaims);

			Response.Cookies.Append("AuthCookie", EncryptedClaims);

			return Ok(Redirect("main"));
		}

	}
}
