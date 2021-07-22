using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.ValidateCode
{
    internal class GetValidateDataService : IGetService<GetValidateDataQuery, string?>
    {
        private readonly IValidateCodeStore _validateCodeStore;

        public GetValidateDataService(IValidateCodeStore validateCodeStore)
        {
            _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));
        }

        public ValueTask<string?> GetAsync(GetValidateDataQuery query, CancellationToken cancellationToken = default)
        {
            return _validateCodeStore.GetDataAsync(query.ValidateCode, query.Token);
        }
    }
}