using Common.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using UMServer.Models;
using UMServer.Services;

namespace UMServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService mPlanService;
        public PlansController(IPlanService planService)
        {
            mPlanService = planService;
        }
        [HttpGet]
        public async Task<string> GetPlanInfo(string userid)
        {
            var result=await mPlanService.GetPlanInfo();
            return JsonConvert.SerializeObject(result);
        }

        [HttpPost("create")]
        public async Task<string> CreatePlan([FromBody] Plan plan)
        {
            var result = await mPlanService.CreatePlan(plan);
            return JsonConvert.SerializeObject(result);
        }

        [HttpPut("{id}")]
        public async Task<string> UpdatePlan(int id, Plan plan)
        {
            var result = await mPlanService.UpdatePlan(id,plan);
            return JsonConvert.SerializeObject(result);
        }
    }
}
