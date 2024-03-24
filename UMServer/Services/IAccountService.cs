using Common.Data;
using System.Threading.Tasks;

namespace UMServer.Services
{
	public interface IAccountService
	{
		Task<ApiResult> Register(RegistrationMetadata registerMetadata);
		Task<ApiResult> Activate(string userid, string licenseKey);
		Task<ApiResult> Deactivate(string userid);
		Task<ApiResult> GetAccountInfo(string userid);
		Task<ApiResult> StartTrial(TrialMetadata metadata);
	}
}