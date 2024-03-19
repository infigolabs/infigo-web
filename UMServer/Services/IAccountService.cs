using Common.Data;
using System.Threading.Tasks;

namespace UMServer.Services
{
	public interface IAccountService
	{
		Task<ApiResult> Register(RegistrationMetadata registerMetadata);
        Task<ApiResult> Validate(string userid, string licenseKey);

		Task<ApiResult> GetAccountInfos(string userid);
		Task<ApiResult> StartTrials(TrialMetadata metadata);



    }
}