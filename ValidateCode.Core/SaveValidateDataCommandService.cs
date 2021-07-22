using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.ValidateCode
{
    internal class SaveValidateDataCommandService : ICommandService<SaveValidateDataCommand>
    {
        private readonly IValidateCodeStore _validateCodeStore;

        public SaveValidateDataCommandService(IValidateCodeStore validateCodeStore)
        {
            _validateCodeStore = validateCodeStore ?? throw new ArgumentNullException(nameof(validateCodeStore));
        }

        public async ValueTask ExecuteAsync(SaveValidateDataCommand command, CancellationToken cancellationToken = default)
        {
            var result = await _validateCodeStore
                .SaveDataAsync(command.ValidateCode, command.Token, command.Content)
                .ConfigureAwait(false);

            if (!result)
                throw new ValidateCodeConflictException();
        }
    }
}
