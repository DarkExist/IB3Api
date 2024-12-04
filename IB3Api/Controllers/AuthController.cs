using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using IB3Api.App.Interfaces.Services;
using IB3Api.Api;
using System.Net;
using IB3Api.Core.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using IB3Api.Infrastructure.DTO;
using ErrorOr;
using System.Text.RegularExpressions;
using IB3Api.App.Models.CustomClaim;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace IB3Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ILogger<AuthController> _logger;
		private readonly IUserService _userService;
		private readonly IEncryptionService _encryptionService;
		private readonly SHA256 sha256 = SHA256.Create();

		public AuthController(ILogger<AuthController> logger,
			IUserService userService,
			IEncryptionService encryptionService)
		{
			_logger = logger;
			_userService = userService;
			_encryptionService = encryptionService;
		}
		
		[HttpPost("register")]
		public async Task<ActionResult> PostRegister([FromBody] Dictionary<string, string> data)
		{
			BigInteger p, g, A, B, secretKey;
			string username, password;
			RegisterDTO registerDTO;

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

			try
			{
				string EncodedAuth = data["EncodedAuth"];
				registerDTO =
					JsonConvert.DeserializeObject<RegisterDTO>
					(_encryptionService.Decrypt(EncodedAuth, secretKey.ToString()))!;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}




			username = registerDTO.Username;
			password = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF32.GetBytes(registerDTO.Password))).Replace("-", "").ToLower(); //hash
			var errorOrUser = await _userService.GetUserByNameAsync(username, CancellationToken.None);
			if (!errorOrUser.IsError)
				return StatusCode(398, Error.Failure("Username taken"));

			User user = new();
			user.Name = username;
			user.Password = password;

			var errorOrSuccess = await _userService.CreateUserAsync(user, CancellationToken.None);
			if (errorOrSuccess.IsError)
				return StatusCode(500, Error.Failure(errorOrSuccess.ToString()));

			var userOrError = await _userService.GetUserByNameAsync(user.Name, CancellationToken.None);
			if (userOrError.IsError)
				return StatusCode(500, Error.Failure(userOrError.ToString()));

			if (username == "admin")
				await _userService.AddRoleAsync(userOrError.Value.Id, "ADMIN", CancellationToken.None);

			await _userService.AddRoleAsync(userOrError.Value.Id, "USER", CancellationToken.None);

			return Ok();
		}


		[HttpPost("login")]
		public async Task<ActionResult> PostLogin([FromBody] Dictionary<string, string> data)
		{
			BigInteger p, g, A, B, secretKey;
			string username, password;
			LoginDTO loginDTO;

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

			try
			{
				string EncodedAuth = data["EncodedAuth"];
				loginDTO =
					JsonConvert.DeserializeObject<LoginDTO>
					(_encryptionService.Decrypt(EncodedAuth, secretKey.ToString()))!;
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

			username = loginDTO.Username;
			password = BitConverter.ToString(sha256.ComputeHash(Encoding.UTF32.GetBytes(loginDTO.Password))).Replace("-", "").ToLower();

			var errorOrUser = await _userService.GetUserByNameAsync(username, CancellationToken.None);
			if (errorOrUser.IsError)
				return StatusCode(397); //no such user

			User user = errorOrUser.Value;
			if (password != user.Password)
				return StatusCode(397); // wrong password

			var claims = new List<CustomClaim>()
			{
				new CustomClaim("username", user.Name),
				new CustomClaim("exp",
					DateTime.UtcNow.AddDays(1).ToString("u"))
			};

			//"yyyy-MM-ddTHH:mm:ssZ"

			string jsonClaims = JsonConvert.SerializeObject(claims);

			string EncryptedClaims = _encryptionService.Encrypt(jsonClaims);

			Response.Cookies.Append("AuthCookie", EncryptedClaims);

			return Ok();
		}

		[HttpGet("logout")]
		public async Task<ActionResult> PostLogout()
		{
			BigInteger p, g, A, B, secretKey;
			string encryptedCookie, username, decryptedData;
			List<CustomClaim> claims;

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
					string decryptedStringClaims = _encryptionService.Decrypt(encryptedCookie);
					claims = JsonConvert.DeserializeObject<List<CustomClaim>>(decryptedStringClaims);
					foreach (var claim in claims)
					{
						if (claim.Name == "exp")
						{
							claim.Value = DateTime.UtcNow.AddDays(-1).ToString("u");
						}
					}
				}
				catch (Exception ex)
				{
					return Unauthorized(Error.Failure("No cookies"));
				}
			}
			else
				return Unauthorized(Error.Failure("No cookies"));

			string jsonClaims = JsonConvert.SerializeObject(claims);

			string EncryptedClaims = _encryptionService.Encrypt(jsonClaims);

			Response.Cookies.Append("AuthCookie", EncryptedClaims);

			return Ok();
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
	}
}
