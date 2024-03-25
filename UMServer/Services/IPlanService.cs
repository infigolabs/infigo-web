using Common.Data;
using System.Threading.Tasks;

namespace UMServer.Services
{
	public interface IPlanService
    {
        public Task<ApiResult> GetPlanInfo();
        public Task<ApiResult> CreatePlan(PlanMetadata metadata);
        public Task<ApiResult> UpdatePlan(PlanMetadata metadata);
    }
}
