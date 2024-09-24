
using Tgs.ServiceUtility;

namespace TGS.Services.ValidateCode;

internal class CheckValidateDataExistedService(IValidateCodeStore validateCodeStore) : IGetService<CheckValidateDataExistedQuery, bool?>
{
	private readonly IValidateCodeStore _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));

	public ValueTask<bool?> GetAsync(CheckValidateDataExistedQuery query, CancellationToken cancellationToken = default)
	{
		return _validateCodeStore.CheckDataExistedAsync(query.ValidateCode, query.Token);
	}
}
