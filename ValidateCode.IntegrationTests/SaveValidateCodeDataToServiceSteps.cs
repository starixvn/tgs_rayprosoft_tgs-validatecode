using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework.Legacy;
using TechTalk.SpecFlow;
using TGS.Services.ValidateCode;
using TGS.Services.ValidateCode.Clients;

namespace TGS.ValidateCode.IntegrationTests;

[Binding]
public class SaveValidateCodeDataToServiceSteps(ScenarioContext context)
{
	private readonly ScenarioContext _context = context;
	private ValidateCodeServiceClient _validateCodeServiceClient = default!;

	[BeforeScenario]
	[Scope(Tag = "Success")]
	public void BeforeScenario()
	{
		var testApp = new TestValidateCodeApplication(hostBuilder => hostBuilder
			.ConfigureServices(services =>
			{
				_ = services
					.AddSingleton(sp =>
					{
						var fakeStore = Substitute.For<IValidateCodeStore>();

						_ = fakeStore
							.SaveDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
							.Returns(true);

						return fakeStore;
					});
			}));

		_validateCodeServiceClient = new ValidateCodeServiceClient(testApp.CreateClient());
	}

	[BeforeScenario]
	[Scope(Tag = "Fail")]
	public void BeforeScenarioSetupFailEnv()
	{
		var testApp = new TestValidateCodeApplication(hostBuilder => hostBuilder
			.ConfigureServices(services =>
			{
				_ = services
					.AddSingleton(sp =>
					{
						var fakeStore = Substitute.For<IValidateCodeStore>();

						_ = fakeStore
							.SaveDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
							.Returns(false);

						return fakeStore;
					});
			}));

		_validateCodeServiceClient = new ValidateCodeServiceClient(testApp.CreateClient());
	}

	[Given(@"playerCode is ""(.*)""")]
	public void GivenPlayerCodeIs(string playerCode) => _context.Add("playerCode", playerCode);

	[Given(@"token is ""(.*)""")]
	public void GivenTokenIs(string token) => _context.Add("token", token);

	[Given(@"validate code is ""(.*)""")]
	public void GivenValidateCodeIs(string validateCode) => _context.Add("validateCode", validateCode);

	[Then(@"operation fail")]
	public void ThenOperationFail() => ClassicAssert.IsFalse(_context.Get<bool>("result"));

	[Then(@"operation success")]
	public void ThenOperationSuccess() => ClassicAssert.IsTrue(_context.Get<bool>("result"));

	[When(@"I call save data")]
	public async Task WhenICallSaveDataAsync()
	{
		var token = _context.Get<string>("token");
		var validateCode = _context.Get<string>("validateCode");
		var playerCode = _context.Get<string>("playerCode");

		var result = await _validateCodeServiceClient
			.SaveValidateCodeDataAsync(validateCode, token, JsonSerializer.Serialize(new { playerCode }));

		_context.Set(result, "result");
	}
}
