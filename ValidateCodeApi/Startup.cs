#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TGS.Services.ValidateCode;

namespace TGS.Apis.ValidateCodeApi
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				_ = app.UseDeveloperExceptionPage();
			}

			_ = app.UseSwagger()
				.UseSwaggerUI(
					options => options.SwaggerEndpoint(
						"/swagger/v1/swagger.json",
						"TGS ValidateCode API"))
				.UseRouting();

			_ = app.UseEndpoints(endpoints =>
			{
				_ = endpoints.MapControllers();
				_ = endpoints.MapHealthChecks("/health");
			});
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			_ = services
				.AddValidateCode()
				.UseRedis((options, sp) =>
				{
					var config = sp.GetRequiredService<IConfiguration>();

					options.ConnectionString = config.GetConnectionString("RedisConfigurationString");
				});

			_ = services
				.AddControllers()
				.AddNewtonsoftJson();

			_ = services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc(
					"v1",
					new()
					{
						Title = "TGS ValidateCode API",
						Version = "v1"
					});

				var path = Path.Combine(AppContext.BaseDirectory, "ValidateCodeApi.xml");
				if (File.Exists(path))
					c.IncludeXmlComments(path);
			});

			_ = services.AddHealthChecks();
		}
	}
}

#pragma warning restore CS1591 // 遺漏公用可見類型或成員的 XML 註解
