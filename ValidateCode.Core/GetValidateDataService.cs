using Tgs.ServiceUtility;

namespace TGS.Services.ValidateCode;

internal class GetValidateDataService(IValidateCodeStore validateCodeStore) : IGetService<GetValidateDataQuery, string?>
{
	private readonly IValidateCodeStore _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));

	public ValueTask<string?> GetAsync(GetValidateDataQuery query, CancellationToken cancellationToken = default)
	{
		return _validateCodeStore.GetDataAsync(query.ValidateCode, query.Token);
	}
}
