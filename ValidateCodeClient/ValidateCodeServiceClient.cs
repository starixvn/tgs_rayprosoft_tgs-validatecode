using System.Net;
using System.Net.Mime;
using System.Text;

namespace TGS.Services.ValidateCode.Clients;

internal class ValidateCodeServiceClient : IValidateCodeServiceClient
{
	private readonly HttpClient _httpClient;

	public ValidateCodeServiceClient(HttpClient httpClient)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
	}

	public async ValueTask<string?> GetValidateCodeDataAsync(string validateCode, string token)
	{
		var response = await _httpClient
			.GetAsync($"/validate-code/{validateCode}/{token}")
			.ConfigureAwait(false);

		return response.StatusCode != HttpStatusCode.OK
			? null
			: await response.Content.ReadAsStringAsync().ConfigureAwait(false);
	}

	public async ValueTask<bool> SaveValidateCodeDataAsync(string validateCode, string token, string content)
	{
		var body = new StringContent(content, Encoding.UTF8, MediaTypeNames.Application.Json);
		var response = await _httpClient
			.PostAsync($"/validate-code/{validateCode}/{token}", body)
			.ConfigureAwait(false);

		return response.StatusCode == HttpStatusCode.Created;
	}
}
