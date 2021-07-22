using System;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.Services.ValidateCode.Configuration
{
    public class ValidateCodeConfiguration
    {
        public IServiceCollection Services { get; }

        public ValidateCodeConfiguration(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public ValidateCodeConfiguration UseRepository<TRepository>() where TRepository : class, IValidateCodeStore
        {
            Services.AddTransient<IValidateCodeStore, TRepository>();
            return this;
        }
    }
}
