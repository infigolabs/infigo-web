using Common.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UMServer.Models;

namespace UMServer.Services
{
    public class PlanService : IPlanService
    {
        private readonly IDbOperations<Plan> _plan;

        public PlanService(IDbOperations<Plan> plan)
        {
            _plan = plan;
        }

        public async Task<ApiResult> CreatePlan(Plan plan)
        {
           var entity=await _plan.Insert(plan);
           var result = new ApiResult();
            result.StatusCode = (int)StatusCodes.Success;
            result.Data = JsonConvert.SerializeObject(entity);
            return result;
        }

        public async Task<ApiResult> GetPlanInfo()
        {
            var result = new ApiResult();
            var users = await _plan.GetAllAsync();
            result.StatusCode = (int)StatusCodes.Success;
            result.Data = JsonConvert.SerializeObject(users);
            return result;
        }

        public async Task<ApiResult> UpdatePlan(int id, Plan plan)
        {
            var response = await _plan.Update(plan);
            var result = new ApiResult();
            result.StatusCode = (int)StatusCodes.Success;
            return result;
        }
    }
}
