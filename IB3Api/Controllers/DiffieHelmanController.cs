using IB3Api.App.Interfaces.Services;
using IB3Api.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Numerics;

namespace IB3Api.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DiffieHelmanController : ControllerBase
	{
		private readonly ILogger<WeatherForecastController> _logger;
		private readonly IDiffieHelmanService _diffieHelmanService;

		public DiffieHelmanController(ILogger<WeatherForecastController> logger,
			IDiffieHelmanService diffieHelmanService)
		{
			_logger = logger;
			_diffieHelmanService = diffieHelmanService;
		}

		[HttpGet(Name = "GetSecretKey")]
		public async Task<ActionResult<Tuple<BigInteger, BigInteger, BigInteger>>> GetSecretKey()
		{
			BigInteger p = _diffieHelmanService.GenerateRandomPrime(64);
			BigInteger g = _diffieHelmanService.FindPrimitiveRoot(p);
			BigInteger a = _diffieHelmanService.GetRandomBigInteger(64);
			BigInteger A = BigInteger.ModPow(g, a, p);


			Tuple<BigInteger, BigInteger, BigInteger, BigInteger> key =
				Tuple.Create(p, g, A, BigInteger.Zero);

			Tuple<BigInteger, BigInteger> value = Tuple.Create(a, BigInteger.Zero);

			DataHolder.SecurityKeysHolder[key] = value;

			return Ok(Tuple.Create(g, p, A));
		}


		[HttpGet(Name = "GetSecretKey")]
		public ActionResult GetSecretKey(BigInteger p, BigInteger g, BigInteger A, BigInteger B)
		{
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

			return Ok();
		}

	}
}

