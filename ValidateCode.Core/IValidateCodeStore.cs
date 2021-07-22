using System.Threading.Tasks;

namespace TGS.Services.ValidateCode
{
    public interface IValidateCodeStore
    {
        ValueTask<bool> SaveDataAsync(string validateCode, string token, string content);

        ValueTask<string?> GetDataAsync(string validateCode, string token);
    }
}
