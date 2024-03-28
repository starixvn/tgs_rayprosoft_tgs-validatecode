using Tgs.ServiceUtility;

namespace TGS.Services.ValidateCode;

internal class SaveValidateDataCommandService(IValidateCodeStore validateCodeStore) : ICommandService<SaveValidateDataCommand>
{
	private readonly IValidateCodeStore _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));

	public async ValueTask ExecuteAsync(SaveValidateDataCommand command, CancellationToken cancellationToken = default)
	{
		var result = await _validateCodeStore
			.SaveDataAsync(command.ValidateCode, command.Token, command.Content)
			.ConfigureAwait(false);

		if (!result)
			throw new ValidateCodeConflictException();
	}
}
