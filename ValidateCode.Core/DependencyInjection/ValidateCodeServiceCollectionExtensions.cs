using TGS.Services.ValidateCode.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ValidateCodeServiceCollectionExtensions
{
	public static ValidateCodeConfiguration AddValidateCode(this IServiceCollection services)
	{
		_ = services.RegisterAllServiceUtilityTypes(
			typeof(ValidateCodeServiceCollectionExtensions).Assembly,
			ServiceLifetime.Transient);

		var configuration = new ValidateCodeConfiguration(services);

		return configuration;
	}
}
