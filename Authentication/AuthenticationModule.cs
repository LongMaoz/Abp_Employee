using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Volo.Abp.Modularity;
using Volo.Abp;
using Volo.Abp.Swashbuckle;

namespace IRentals.Authentication
{
    [DependsOn(typeof(AbpSwashbuckleModule))]
    public class AuthenticationModule : AbpModule
    {
        public async Task<IEnumerable<SecurityKey>> GetKeysFromJwksUri(string jwksUri)
        {
            var configurationManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(jwksUri,
                    new OpenIdConnectConfigurationRetriever());
            var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
            return openIdConfig.SigningKeys;
        }

        public override async Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            IEnumerable<SecurityKey> jwtKeys = await GetKeysFromJwksUri("https://erp.irentals.top/oidc/certs");
            context.Services
                .AddAuthentication(auth =>
                {
                    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddAbpJwtBearer("Bearer", async options =>
                {
                    options.Authority = "https://erp.irentals.top/oidc/";
                    options.Audience = "erp";
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
                        ValidIssuer = "https://erp.irentals.top/oidc/",
                        ValidateAudience = true,
                        ValidAudience = "erp",
                        ValidateLifetime = true, // 验证令牌的有效期
                        ClockSkew = TimeSpan.Zero
                    };
                    options.MapInboundClaims = false;
                });
            ConfigureSwaggerServices(context,context.Services.GetConfiguration());
            await base.ConfigureServicesAsync(context);
        }

        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGen(
                options =>
                {
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
                });
        }

        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            return base.OnApplicationInitializationAsync(context);
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            return base.PreConfigureServicesAsync(context);
        }
    }
}