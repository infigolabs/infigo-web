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
			//var result = mAccountService.StartTrial(trialMetadata);
			var result =await mAccountService.StartTrials(trialMetadata);
			return JsonConvert.SerializeObject(result);
		}

		[HttpGet("user/info")]
		public async Task<string> GetAccountInfo(string userid)
		{
			//var result = mAccountService.GetAccountInfo(userid);
			var result = await mAccountService.GetAccountInfos(userid);
			//var settings = new JsonSerializerSettings
			//{
			//	ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			//};
			return JsonConvert.SerializeObject(result);
		}

		[HttpPost("register")]
		public async Task<string> Register([FromBody] RegistrationMetadata registrationMetadata)
		{
			var result = await mAccountService.Register(registrationMetadata);
			return JsonConvert.SerializeObject(result);
		}

		[HttpPost("subscription/verify")]
		public async Task<string> VerifyLicense([FromBody] LicenseMetadata licenseMetadata)
		{
            var result = await mAccountService.Validate(licenseMetadata.UserId,licenseMetadata.LicenseKey);
            return JsonConvert.SerializeObject(result);
        }
	}
}
