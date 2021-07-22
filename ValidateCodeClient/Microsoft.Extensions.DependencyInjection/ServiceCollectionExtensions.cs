using System;
using Microsoft.Extensions.Options;
using TGS.Services.ValidateCode.Clients;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddValidateCodeClient(
			this IServiceCollection services,
			Action<ValidateCodeClientOptions, IServiceProvider> configure)
			=> services
				.AddOptions<ValidateCodeClientOptions>()
				.Configure(configure)
				.Services
				.AddHttpClient<ValidateCodeServiceClient>()
				.ConfigureHttpClient((sp, httpClient) =>
				{
					var options = sp.GetRequiredService<IOptions<ValidateCodeClientOptions>>().Value;

					httpClient.BaseAddress = options.ValidateCodeBaseUrl;
				})
				.Services
				.AddTransient<IValidateCodeServiceClient>(sp => sp.GetRequiredService<ValidateCodeServiceClient>());
	}
}
