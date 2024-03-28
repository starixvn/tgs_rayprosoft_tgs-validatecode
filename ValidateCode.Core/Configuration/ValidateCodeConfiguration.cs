using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.ValidateCode.Configuration;

public class ValidateCodeConfiguration(IServiceCollection services)
{
	public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException(nameof(services));

	public ValidateCodeConfiguration UseRepository<TRepository>() where TRepository : class, IValidateCodeStore
	{
		_ = Services.AddTransient<IValidateCodeStore, TRepository>();
		return this;
	}
}
