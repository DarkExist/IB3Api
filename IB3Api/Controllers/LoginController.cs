using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IB3Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class LoginController : ControllerBase
	{
		private readonly ILogger<WeatherForecastController> _logger;

		public LoginController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet(Name = "GetLogin")]
		public void Get()
		{
			
		}



		private int expmod(int baseexp, int exp, int mod)
		{
			if (exp == 0) return 1;
			if (exp % 2 == 0)
			{
				return (int)Math.pow(expmod(baseexp, (exp / 2), mod), 2) % mod;
			}
			else
			{
				return (base * expmod(babaseexpe, (exp - 1), mod)) % mod;
			}
		}
	}
}
