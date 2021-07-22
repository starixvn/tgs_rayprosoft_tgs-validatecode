namespace TGS.Services.ValidateCode
{
    public record SaveValidateDataCommand(string ValidateCode, string Token, string Content);
}
