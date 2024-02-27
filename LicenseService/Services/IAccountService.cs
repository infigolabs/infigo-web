using LicenseService.Payloads;

namespace LicenseService.Services
{
	public interface IAccountService
	{
		ApiResult StartTrial(TrialMetadata trialMetadata);
		ApiResult GetAccountInfo(string userid);
		ApiResult Register(string userid, string planId, string email, string name);
		ApiResult Validate(string userid, string licenseKey);
	}
}