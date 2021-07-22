using System.Linq;
using TGS.Services.ValidateCode.Configuration;
using TGS.ServiceUtility;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ValidateCodeServiceCollectionExtensions
    {
        public static ValidateCodeConfiguration AddValidateCode(this IServiceCollection services)
        {
            services.RegisterAllServiceUtilityTypes(
                typeof(ValidateCodeServiceCollectionExtensions).Assembly,
                ServiceLifetime.Transient);

            var configuration = new ValidateCodeConfiguration(services);

            return configuration;
        }
    }
}
