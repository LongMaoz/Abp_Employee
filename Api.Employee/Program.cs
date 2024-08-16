using Api.Employee;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseAutofac();
builder.Configuration.AddJsonFile("appsettings.json").AddEnvironmentVariables();
await builder.Services.AddApplicationAsync<ApiEmployeeModule>();
var app = builder.Build();
await app.InitializeApplicationAsync();
await app.RunAsync();