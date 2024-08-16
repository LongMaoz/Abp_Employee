using Services.EmployeeManagement;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Volo.Abp.AspNetCore;
using Volo.Abp.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Service;
using Application;
using MiddleWare;
using Microsoft.OpenApi.Models;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Swashbuckle;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Application.Exception;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using ApiCore.EmployeeManagement.Converter;
using Domain.Dto.Employee;
using System.Net;
using Employee.GrpcService;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Entity.Base;
using GrpcService;
using Employee.GrpcService.Services;

namespace ApiCore.EmployeeManagement;

/// <summary>
/// 
/// </summary>
[DependsOn(
    typeof(EmployeeGrpcServiceModule),
    typeof(EmployeeManagementServiceModule),
    typeof(ApplicationModule),
    typeof(CustomMiddleWareModule),
    typeof(AppVoloAbpModule),
    typeof(AbpHttpModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreModule),
    typeof(AbpAutofacModule)
    )]
public class ErpApiModule : AbpModule
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>

    public override async Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddEndpointsApiExplorer();
        //错误消息过滤
        context.Services.AddControllers(options =>
        {
            var index = options.Filters.FindIndex(x => x is ServiceFilterAttribute attr && attr.ServiceType == typeof(AbpExceptionFilter));
            if (index > -1)
                options.Filters.RemoveAt(index);
            options.Filters.Add(typeof(CustomValidationExceptionHandler));
        }).AddNewtonsoftJson(options =>
        {
            //首字母小写
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.Converters.Add(new CustomStringEnumConverter());
        });

        //swagger authentication
        ConfigureSwaggerServices(context, context.Services.GetConfiguration());
        //配置identity4后再启动
        //await ConfigureAuthentication(context, context.Services.GetConfiguration());

        //模型验证
        context.Services.Configure<MvcOptions>(options =>
        {
            options.ModelMetadataDetailsProviders.Clear();
            options.ModelValidatorProviders.Clear();
        });
        //Configure<AbpAspNetCoreMvcOptions>(options =>
        //{
        //    options.ConventionalControllers.Create(typeof(IRentalsApplicationModule).Assembly);
        //});
        await base.ConfigureServicesAsync(context);
    }

    private static async Task ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var configurationManager =
            new ConfigurationManager<OpenIdConnectConfiguration>(configuration["AuthServer:Jwks"],
                new OpenIdConnectConfigurationRetriever());
        var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
        IEnumerable<SecurityKey> jwtKeys = openIdConfig.SigningKeys;
        context.Services
            .AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAbpJwtBearer("Bearer", async options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.Audience = configuration["AuthServer:Audience"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, securityToken, identifier, parameters) =>
                    {
                        var keys = jwtKeys;
                        return keys;
                    },
                    ValidateIssuer = true,
                    ValidIssuer = configuration["AuthServer:Authority"],
                    ValidateAudience = true,
                    ValidAudience = "erp",
                    ValidateLifetime = true, // 验证令牌的有效期
                    ClockSkew = TimeSpan.Zero
                };
                options.MapInboundClaims = false;
            });
    }

    private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAbpSwaggerGen(
            options =>
            {
                var urls = new List<SwaggerModel>();
                configuration.GetSection("SwaggerUI:BaseUrl").Bind(urls);
                urls.ForEach(item =>
                {
                    options.AddServer(new OpenApiServer() { Url = item.Url, Description = item.Des });
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                options.HideAbpEndpoints();
                options.EnableAnnotations();
                
                var assembly = typeof(ErpApiModule).Assembly;
                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
            });
        context.Services.AddSwaggerGenNewtonsoftSupport();
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<GrpcEmployeesService>();
            endpoints.MapGrpcService<GrpcEmployeeRoleService>();
            endpoints.MapGrpcService<GrpcEmployeeGroupsService>();
            endpoints.MapControllers();
        });
        app.UseStaticFiles();
        app.UseSwagger();
        app.UseAbpSwaggerUI(x =>
        {
            string swaggerJsonBasePath = string.IsNullOrWhiteSpace(x.RoutePrefix) ? "." : "..";
            x.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Employee API");
        });
        app.UseConfiguredEndpoints();
        app.UseHttpsRedirection();
        return base.OnApplicationInitializationAsync(context);
    }
}