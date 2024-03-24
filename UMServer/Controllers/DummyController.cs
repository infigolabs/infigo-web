using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using UMServer.Common;
using UMServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UMServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        private readonly ApplicationDBContext DBContext;

        public DummyController(ApplicationDBContext DBContext)
        {
            this.DBContext = DBContext;
        }

        // GET: api/<DummyController>
        [HttpGet]
        public IEnumerable<string> Get()    
        
        {
            //AddDummydata();
            return new string[] { "value1", "value2" };
        }

        // GET api/<DummyController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DummyController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DummyController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DummyController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        //private void AddDummydata()
        //{    
        //        // Add a dummy row to the plans table
        //        var dummyPlan = new Plan
        //        {
        //            PlanLength = 30,
        //            PlanPrice = 9.99m,
        //            PlanDescription = "Dummy Plan"
        //        };
        //        //DBContext.Plans.Add(dummyPlan);
        //        //DBContext.SaveChanges();

        //        // Add a dummy row to the users table
        //        var dummyUser = new Account
        //        {
        //            PlanId = 1,
        //            LicenseKey = "ABC123",
        //            Email = "dummy@example.com",
        //            ProductVersion = "1.0",
        //            SubscriptionStatus = "Active",
        //            SubscriptionStart = TimeUtils.Now(),
        //            SubscriptionEnd = TimeUtils.Now() + 14*24*60*60,
        //            Active = true,
        //            IsExpired = false,
        //            IsDeviceActive = true
        //        };
        //        //DBContext.Users.Add(dummyUser);
        //        //DBContext.SaveChanges();

        //        // Add a dummy row to the premium_users table
        //        var dummyPremiumUser = new PremiumUser
        //        {
        //            LicenseKey = "ABC123",
        //            PlanId = 1
        //        };
        //        //DBContext.PremiumUsers.Add(dummyPremiumUser);
        //        //DBContext.SaveChanges();

        //        // Add a dummy row to the userdetails table
        //        var dummyUserDetail = new AccountDetail
        //        {
        //            UserId = 1,
        //            Name = "Dummy User",
        //            OS = "Windows",
        //            OSVersion = "10",
        //            Country = "USA"
        //        };
        //        DBContext.UserDetails.Add(dummyUserDetail);
        //        DBContext.SaveChanges();
        //    }
        
    }
}
