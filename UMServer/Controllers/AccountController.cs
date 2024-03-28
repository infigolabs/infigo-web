using Common.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UMServer.Services;

namespace UMServer.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AccountController : Controller
	{
		private readonly IAccountService mAccountService;

		public AccountController(IAccountService accountService)
		{
			mAccountService = accountService;
		}

		[HttpPost("trial")]
		public async Task<string> StartTrial([FromBody] TrialMetadata trialMetadata)
		{
			var result =await mAccountService.StartTrial(trialMetadata);
			return JsonConvert.SerializeObject(result);
		}

		[HttpGet("user/info")]
		public async Task<string> GetAccountInfo(string userid)
		{
			var result = await mAccountService.GetAccountInfo(userid);
			return JsonConvert.SerializeObject(result);
		}

		[HttpPost("register")]
		public async Task<string> Register([FromBody] RegistrationMetadata registrationMetadata)
		{
			var result = await mAccountService.Register(registrationMetadata);
			return JsonConvert.SerializeObject(result);
		}

		[HttpPost("activate")]
		public async Task<string> Activate([FromBody] LicenseMetadata licenseMetadata)
		{
            var result = await mAccountService.Activate(licenseMetadata.UserId,licenseMetadata.LicenseKey);
            return JsonConvert.SerializeObject(result);
        }

		[HttpGet("deactivate")]
		public async Task<string> Deactivate(string userid)
		{
			var result = await mAccountService.Deactivate(userid);
			return JsonConvert.SerializeObject(result);
		}
	}
}
