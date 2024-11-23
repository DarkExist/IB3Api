using IB3Api.App.Interfaces.Services;
using IB3Api.Controllers;
using IB3Api.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace IB3Api.Api.Controllers
{
	public class ProfileController : ControllerBase
	{
		private readonly ILogger<LoginController> _logger;
		private readonly IProfileService _profileService;
		private readonly IUserService _userService;

		public ProfileController(ILogger<LoginController> logger,
			IProfileService profileService,
			IUserService userService)
		{
			_logger = logger;
			_profileService = profileService;
			_userService = userService;
		}

		[HttpGet]
		public async Task<ActionResult> GetProfileDescription()
		{
			//check authorize

			//get name from httpreauest

			string username = "1213";

			var errorOrUser = await _userService.GetUserByNameAsync(username, CancellationToken.None);
			if (errorOrUser.IsError)
				return BadRequest(errorOrUser);

			Guid userGuid = errorOrUser.Value.Id;

			var errorOrProfile = await _profileService.GetProfileById(userGuid, CancellationToken.None);
			if (errorOrProfile.IsError)
				return BadRequest(errorOrProfile);

			Profile profile = errorOrProfile.Value;

			return Ok(profile.Description);
		}

		[HttpPost]
		public async Task<ActionResult> UpdateProfileDescription(string newDescription)
		{
			//check authorize

			//get name from httpreauest

			string username = "1213";

			var errorOrUser = await _userService.GetUserByNameAsync(username, CancellationToken.None);
			if (errorOrUser.IsError)
				return BadRequest(errorOrUser);

			Guid userGuid = errorOrUser.Value.Id;

			var errorOrProfile = await _profileService.GetProfileById(userGuid, CancellationToken.None);
			if (errorOrProfile.IsError)
				return BadRequest(errorOrProfile);

			await _profileService.UpdateDescriptionAsync(userGuid, newDescription, CancellationToken.None)

			return Ok();
		}
	}
}
