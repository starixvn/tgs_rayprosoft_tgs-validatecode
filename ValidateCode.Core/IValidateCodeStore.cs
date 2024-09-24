namespace TGS.Services.ValidateCode;

public interface IValidateCodeStore
{
	ValueTask<bool> SaveDataAsync(string validateCode, string token, string content);

	ValueTask<string?> GetDataAsync(string validateCode, string token);

	ValueTask<bool?> CheckDataExistedAsync(string validateCode, string token);
}
