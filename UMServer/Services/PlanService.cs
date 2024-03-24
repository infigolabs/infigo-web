using Common.Data;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using UMServer.Common;
using UMServer.Models;

namespace UMServer.Services
{
	public class PlanService : IPlanService
	{
		private readonly ApplicationDBContext mDBContext;

		public PlanService(ApplicationDBContext dbContext)
		{
			mDBContext = dbContext;
		}

		public async Task<ApiResult> CreatePlan(PlanMetadata metadata)
		{
			var result = new ApiResult();

			try
			{
				Plan plan = mDBContext.Plans.FirstOrDefault(p => p.PlanId == metadata.PlanId);
				if (plan != null)
				{
					throw new Exception("Plan already exist");
				}

				plan = new Plan()
				{
					PlanId = metadata.PlanId,
					PlanDescription = metadata.PlanDescription,
					PlanLength = metadata.PlanLength,
					PlanPrice = metadata.Price
				};
				
				await mDBContext.AddAsync(plan);
				await mDBContext.SaveChangesAsync();
				result.Data = JsonConvert.SerializeObject(metadata);
			}
			catch (Exception ex)
			{
				result.StatusCode = (int)StatusCodes.CreatePlanErr;
				result.Error = ex.Message;
			}
			return result;
		}

		public async Task<ApiResult> GetPlanInfo()
		{
			var result = new ApiResult();
			try
			{ 
				result.Data = JsonConvert.SerializeObject(mDBContext.Plans);
			}
			catch (Exception ex)
			{
				result.StatusCode = (int)StatusCodes.PlanErr;
				result.Error = ex.Message;
			}
			return result;
		}

		public async Task<ApiResult> UpdatePlan(PlanMetadata metadata)
		{
			var result = new ApiResult();
			try
			{
				Plan plan = mDBContext.Plans.FirstOrDefault(p => p.PlanId == metadata.PlanId);
				if (plan == null)
				{
					throw new Exception("Plan not found");
				}

				plan = new Plan()
				{
					PlanId = metadata.PlanId,
					PlanDescription = metadata.PlanDescription,
					PlanLength = metadata.PlanLength,
					PlanPrice = metadata.Price
				};

				mDBContext.Plans.Update(plan);
				await mDBContext.SaveChangesAsync();
				result.Data = JsonConvert.SerializeObject(metadata);
			}
			catch (Exception ex)
			{
				result.StatusCode = (int)StatusCodes.PlanErr;
				result.Error = ex.Message;
			}
			return result;
		}
	}
}
