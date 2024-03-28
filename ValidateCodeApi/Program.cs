using FluentdForward.OpenTelemetry.MessagePack;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TGS.Services.ValidateCode;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack(LogRecordFormatterResolver.GetResolverInstanceWithExtendInfo(
				"MachineName",
				() => Environment.MachineName));
		});
	});

builder.Services
	.AddValidateCode()
	.UseRedis((options, sp) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();

		options.ConnectionString = config.GetConnectionString("RedisConfigurationString")!;
	});

builder.Services
	.AddControllers()
	.AddNewtonsoftJson();

var excludeExtensions = new[] { "html", "ico", "png", "jpg" };
builder.Services
	.AddOpenTelemetry()
	.WithTracing(tracingBuilder => tracingBuilder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddAspNetCoreInstrumentation(options => options.Filter = httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/healthz")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !excludeExtensions.Any(
				ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.AddProfileViewExporter()
		.AddOtlpExporter())
	.WithMetrics(meterBuiler => meterBuiler
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter())
	.Services
	.AddProfileViewer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc(
		"v1",
		new()
		{
			Title = "TGS ValidateCode API",
			Version = "v1"
		});

	var path = Path.Combine(AppContext.BaseDirectory, "ValidateCodeApi.xml");
	if (File.Exists(path))
		c.IncludeXmlComments(path);
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint(
	"/swagger/v1/swagger.json",
	"TGS ValidateCode API"));

app.MapControllers();
app.MapProfileViewer();
app.MapHealthChecks("/healthz");

app.Run();
