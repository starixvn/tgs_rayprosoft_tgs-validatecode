using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NUnit.Framework;
using TechTalk.SpecFlow;
using TGS.Services.ValidateCode;
using TGS.Services.ValidateCode.Clients;

namespace TGS.ValidateCode.IntegrationTests
{
	[Binding]
	public class SaveValidateCodeDataToServiceSteps
	{
		private readonly ScenarioContext _context;
		private IValidateCodeServiceClient _validateCodeServiceClient;

		public SaveValidateCodeDataToServiceSteps(ScenarioContext context)
		{
			_context = context;
		}

		[BeforeScenario]
		[Scope(Tag = "Success")]
		public void BeforeScenario()
		{
			var opApiHost = Host.CreateDefaultBuilder(Array.Empty<string>())
				.ConfigureWebHostDefaults(webBuilder => webBuilder
					.UseStartup<Apis.ValidateCodeApi.Startup>()
					.UseTestServer()
					.ConfigureTestServices(services => services
						.AddSingleton<IValidateCodeStore>(sp =>
						{
							var fakeStore = Substitute.For<IValidateCodeStore>();

							_ = fakeStore
								.SaveDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
								.Returns(true);

							return fakeStore;
						})))
				.Build();

			_ = opApiHost.StartAsync();

			_validateCodeServiceClient = new ValidateCodeServiceClient(opApiHost.GetTestClient());
		}

		[BeforeScenario]
		[Scope(Tag = "Fail")]
		public void BeforeScenarioSetupFailEnv()
		{
			var opApiHost = Host.CreateDefaultBuilder(Array.Empty<string>())
				.ConfigureWebHostDefaults(webBuilder => webBuilder
					.UseStartup<Apis.ValidateCodeApi.Startup>()
					.UseTestServer()
					.ConfigureTestServices(services => services
						.AddSingleton<IValidateCodeStore>(sp =>
						{
							var fakeStore = Substitute.For<IValidateCodeStore>();

							_ = fakeStore
								.SaveDataAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
								.Returns(false);

							return fakeStore;
						})))
				.Build();

			_ = opApiHost.StartAsync();

			_validateCodeServiceClient = new ValidateCodeServiceClient(opApiHost.GetTestClient());
		}

		[Given(@"playerCode is ""(.*)""")]
		public void GivenPlayerCodeIs(string playerCode)
		{
			_context.Add("playerCode", playerCode);
		}

		[Given(@"token is ""(.*)""")]
		public void GivenTokenIs(string token)
		{
			_context.Add("token", token);
		}

		[Given(@"validate code is ""(.*)""")]
		public void GivenValidateCodeIs(string validateCode)
		{
			_context.Add("validateCode", validateCode);
		}

		[Then(@"operation fail")]
		public void ThenOperationFail()
		{
			Assert.IsFalse(_context.Get<bool>("result"));
		}

		[Then(@"operation success")]
		public void ThenOperationSuccess()
		{
			Assert.IsTrue(_context.Get<bool>("result"));
		}

		[When(@"I call save data")]
		public async System.Threading.Tasks.Task WhenICallSaveDataAsync()
		{
			var token = _context.Get<string>("token");
			var validateCode = _context.Get<string>("validateCode");
			var playerCode = _context.Get<string>("playerCode");

			var result = await _validateCodeServiceClient.SaveValidateCodeDataAsync(validateCode, token, JsonSerializer.Serialize(new { playerCode }));

			_context.Set<bool>(result, "result");
		}
	}
}
