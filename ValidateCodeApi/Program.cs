#pragma warning disable CS1591 // 遺漏公用可見類型或成員的 XML 註解

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TGS.Apis.ValidateCodeApi
{
	public class Program
	{
		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration(builder => builder
					.AddJsonFile("/config/appsettings.json", true))
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}
	}
}

#pragma warning restore CS1591 // 遺漏公用可見類型或成員的 XML 註解
