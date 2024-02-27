using UMServer.Payloads;

namespace UMServer.Services
{
	public interface IAccountService
	{
		ApiResult StartTrial(TrialMetadata trialMetadata);
		ApiResult GetAccountInfo(string userid);
		ApiResult Register(RegistrationMetadata registerMetadata);
		ApiResult Validate(string userid, string licenseKey);
	}
}