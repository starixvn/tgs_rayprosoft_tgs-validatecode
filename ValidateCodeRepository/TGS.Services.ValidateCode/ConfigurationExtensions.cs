using System;
using Microsoft.Extensions.DependencyInjection;
using TGS.Repositories.ValidateCode;
using TGS.Services.ValidateCode.Configuration;

namespace TGS.Services.ValidateCode
{
	public static class ConfigurationExtensions
	{
		public static ValidateCodeConfiguration UseRedis(
			this ValidateCodeConfiguration configuration,
			Action<RedisOptions, IServiceProvider> configure)
		{
			configuration.Services
				.AddOptions<RedisOptions>()
				.Configure(configure)
				.Services
				.AddSingleton<RedisConnectionFactory>();

			return configuration
				.UseRepository<RedisValidateCodeStore>();
		}
	}
}
