
using System.Net;
using Autofac.Extensions.DependencyInjection;
using Services.EmployeeManagement.Interface;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using Feather.SerilogAliyun.LogService;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Employee.GrpcService.Services;

namespace ApiCore.EmployeeManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            configuration.AddJsonFile("appsettings.json", true).AddEnvironmentVariables();
            //LogClientConfig logClientConfig = new();
            //configuration.GetSection("LogClientConfig").Bind(logClientConfig);
            //logClientConfig.StoreName = configuration["LogClientConfig:LogStoreNameErp"];
            var env = builder.Environment;
            Console.WriteLine($"env:{env.EnvironmentName}");
            //ÅäÖÃºóÔÙ¼ÓÔØ
            //builder.Configuration.AddNacosV2Configuration(configuration.GetSection($"NacosConfig{env.EnvironmentName}"));
            //builder.Configuration.AddNacosV2Configuration(configuration.GetSection($"NacosConfigGrpc{env.EnvironmentName}"), parser: Nacos.YamlParser.YamlConfigurationStringParser.Instance);
            builder.WebHost
                .UseKestrel(options => options.Limits.MaxRequestBodySize = null)
                .ConfigureKestrel((context, options) =>
                {
                    options.Listen(IPAddress.Any, 8080, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1;
                    });
                    options.Listen(IPAddress.Any, 8081, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                });
            //.CreateSerilogAndAliyun(configuration, logClientConfig);
            builder.Host.UseAutofac();//.UseSerilog();
            await builder.Services.AddApplicationAsync<ErpApiModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
        }
    }
}
