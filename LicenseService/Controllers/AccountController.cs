﻿using LicenseService.Payloads;
using LicenseService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LicenseService.Controllers
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
		public string StartTrial([FromBody] TrialMetadata trialMetadata)
		{
			var result = mAccountService.StartTrial(trialMetadata);
			return Newtonsoft.Json.JsonConvert.SerializeObject(result);
		}

		[HttpGet("user/info")]
		public string GetAccountInfo(string userid)
		{
			var result = mAccountService.GetAccountInfo(userid);
			return Newtonsoft.Json.JsonConvert.SerializeObject(result);
		}

		[HttpPost("register")]
		public bool Register([FromBody] RegistrationMetadata registrationMetadata)
		{
			var result = mAccountService.Register();
			return Newtonsoft.Json.JsonConvert.SerializeObject(result);
		}

		[HttpPost("subscription/verify")]
		public bool VerifyLicense([FromBody] LicenseMetadata licenseMetadata)
		{
			return true;
		}
	}
}
