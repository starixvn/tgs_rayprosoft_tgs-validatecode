namespace TGS.Services.ValidateCode.Clients;

public interface IValidateCodeServiceClient
{
	ValueTask<string?> GetValidateCodeDataAsync(string validateCode, string token);

	ValueTask<bool> SaveValidateCodeDataAsync(string validateCode, string token, string content);

	ValueTask<bool> CheckValidateCodeExistedAsync(string validateCode, string token);
}
