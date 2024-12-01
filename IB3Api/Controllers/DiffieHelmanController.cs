using IB3Api.App.Interfaces.Services;
using IB3Api.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IB3Api.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class DiffieHelmanController : ControllerBase
	{
		private readonly ILogger<DiffieHelmanController> _logger;
		private readonly IDiffieHelmanService _diffieHelmanService;

		public DiffieHelmanController(ILogger<DiffieHelmanController> logger,
			IDiffieHelmanService diffieHelmanService)
		{
			_logger = logger;
			_diffieHelmanService = diffieHelmanService;
		}

		[HttpGet]
		public async Task<ActionResult<string>> GetSecretKey()
		{
			BigInteger p = _diffieHelmanService.GenerateRandomPrime(54);
			BigInteger g = _diffieHelmanService.FindPrimitiveRoot(p);
			BigInteger a = _diffieHelmanService.GetRandomBigInteger(54);
			BigInteger A = BigInteger.ModPow(g, a, p);


			Tuple<BigInteger, BigInteger, BigInteger, BigInteger> key =
				Tuple.Create(p, g, A, BigInteger.Zero);

			Tuple<BigInteger, BigInteger> value = Tuple.Create(a, BigInteger.Zero);

			DataHolder.SecurityKeysHolder[key] = value;
			
			return Ok(JsonConvert.SerializeObject(Tuple.Create(p, g, A)));
		}


		[HttpPost]
		public ActionResult GetSecretKey([FromBody] Dictionary<string, string> data)
		{
			BigInteger p, g, A, B;
			try
			{
				p = BigInteger.Parse(data["p"]);
				g = BigInteger.Parse(data["g"]);
				A = BigInteger.Parse(data["A"]);
				B = BigInteger.Parse(data["B"]);
			}
			catch
			{
				return BadRequest("Wrong format");
			}

			Tuple<BigInteger, BigInteger, BigInteger, BigInteger> keyOld =
				Tuple.Create(p, g, A, BigInteger.Zero);

			Tuple<BigInteger, BigInteger, BigInteger, BigInteger> keyNew =
				Tuple.Create(p, g, A, B);

			if (!DataHolder.SecurityKeysHolder.ContainsKey(keyOld))
				return BadRequest("Try again");

			BigInteger a = DataHolder.SecurityKeysHolder[keyOld].Item1;
			BigInteger secret = BigInteger.ModPow(B, a, p);

			Tuple<BigInteger, BigInteger> valueNew =
				Tuple.Create(a, secret);

			DataHolder.SecurityKeysHolder.TryAdd(keyNew, valueNew);

			DataHolder.SecurityKeysHolder.TryRemove(keyOld,
				out Tuple<BigInteger, BigInteger> removedItem);

			_logger.LogDebug($"Created secret key {secret}");
			return Ok();
		}

	}
}

