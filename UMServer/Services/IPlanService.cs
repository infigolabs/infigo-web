using Common.Data;
using System.Threading.Tasks;
using UMServer.Models;

namespace UMServer.Services
{
    public interface IPlanService
    {
        public Task<ApiResult> GetPlanInfo();
        public Task<ApiResult> CreatePlan(Plan plan);

        public Task<ApiResult> UpdatePlan(int id,Plan plan);
    }
}
