using EntityFrameworkCore;
using IdentityServer.Custom;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace IdentityServer
{
    [DependsOn(
        typeof(EmployeeManagementDataModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAspNetCoreModule),
        typeof(AbpAutofacModule)
    )]
    public class IdentityServerModule : AbpModule
    {
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddEndpointsApiExplorer();
            context.Services.AddControllers();
            context.Services.AddSwaggerGen();

            var signingCredentials = LoadSigningCredentials("./jwks.json");

            var identityServerBuilder = context.Services.AddIdentityServer()
                //.AddDeveloperSigningCredential() // 开发环境密钥
                .AddInMemoryApiResources(ClientConfig.ApiResources)
                .AddInMemoryApiScopes(ClientConfig.ApiScopes)
                .AddInMemoryClients(ClientConfig.Clients)
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();

            foreach (var credential in signingCredentials)
            {
                identityServerBuilder.AddSigningCredential(credential);
            }
            return base.ConfigureServicesAsync(context);
        }

        public override Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            app.UseRouting();
            if (env.IsDevelopment())
            {
                app.UseStaticFiles();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return base.OnPreApplicationInitializationAsync(context);
        }

        private IEnumerable<SigningCredentials> LoadSigningCredentials(string jwksPath)
        {
            var keyManagementService = new KeyManagementService(jwksPath);
            var jwks = keyManagementService.LoadOrCreateKeys();
            var signingCredentials = new List<SigningCredentials>();

            foreach (var key in jwks.Keys)
            {
                if (key.Use == "sig")
                {
                    if (key.Kty == "RSA")
                    {
                        var rsaKey = new RsaSecurityKey(new RSAParameters
                        {
                            Modulus = Base64UrlEncoder.DecodeBytes(key.N),
                            Exponent = Base64UrlEncoder.DecodeBytes(key.E),
                            D = Base64UrlEncoder.DecodeBytes(key.D), // 私钥部分
                            P = Base64UrlEncoder.DecodeBytes(key.P),
                            Q = Base64UrlEncoder.DecodeBytes(key.Q),
                            DP = Base64UrlEncoder.DecodeBytes(key.DP),
                            DQ = Base64UrlEncoder.DecodeBytes(key.DQ),
                            InverseQ = Base64UrlEncoder.DecodeBytes(key.QI)
                        })
                        {
                            KeyId = key.Kid
                        };
                        signingCredentials.Add(new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256));
                    }
                    else if (key.Kty == "EC")
                    {
                        var curve = key.Crv switch
                        {
                            "P-256" => ECCurve.NamedCurves.nistP256,
                            "P-384" => ECCurve.NamedCurves.nistP384,
                            "P-521" => ECCurve.NamedCurves.nistP521,
                            _ => throw new NotSupportedException($"Unsupported curve: {key.Crv}")
                        };

                        var algorithm = key.Crv switch
                        {
                            "P-256" => SecurityAlgorithms.EcdsaSha256,
                            "P-384" => SecurityAlgorithms.EcdsaSha384,
                            "P-521" => SecurityAlgorithms.EcdsaSha512,
                            _ => throw new NotSupportedException($"Unsupported curve: {key.Crv}")
                        };

                        var ecKey = new ECDsaSecurityKey(ECDsa.Create(new ECParameters
                        {
                            Curve = curve,
                            Q = new ECPoint
                            {
                                X = Base64UrlEncoder.DecodeBytes(key.X),
                                Y = Base64UrlEncoder.DecodeBytes(key.Y)
                            },
                            D = Base64UrlEncoder.DecodeBytes(key.D) // 私钥部分
                        }))
                        {
                            KeyId = key.Kid
                        };
                        signingCredentials.Add(new SigningCredentials(ecKey, algorithm));
                    }
                }
            }

            return signingCredentials;
        }
    }
}
