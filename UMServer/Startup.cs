using UMServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMServer.Common;
using Microsoft.EntityFrameworkCore;

namespace UMServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

            services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "UMServer", Version = "v1" });
			});



			services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

			//services.AddSingleton<IDatabaseService, LicenseDatabaseService>();
			services.AddSingleton<IEmailService, EmailService>();
			services.AddTransient<IAccountService, AccountService>();
			services.AddTransient<IPlanService, PlanService>();

			services.AddDbContext<ApplicationDBContext>(options =>
			{
				if (string.IsNullOrWhiteSpace(Configuration.GetConnectionString("SqlConnection")) == false)
				{
					options.UseSqlServer(Configuration.GetConnectionString("SqlConnection"));
				}
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UMServer v1"));
			}

			app.ApplicationServices.GetService<IDatabaseService>()?.Initialize();
            app.UseCors(builder => builder.AllowAnyHeader()
.AllowAnyMethod().
WithOrigins("http://localhost:3000"));

            app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
