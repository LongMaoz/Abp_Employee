using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public static class ClientConfig
    {

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("erp")
                {
                    Scopes = { IdentityServerConstants.StandardScopes.Profile },
                    ApiSecrets = { new Secret("erp@cn".Sha256()) },
                    UserClaims =
                    {
                        JwtClaimTypes.Name,
                        JwtClaimTypes.Email
                    },
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope(IdentityServerConstants.StandardScopes.OpenId),
                new ApiScope(IdentityServerConstants.StandardScopes.Profile),
                new ApiScope(IdentityServerConstants.StandardScopes.Email),
                new ApiScope(IdentityServerConstants.StandardScopes.Phone),
                new ApiScope("roles"),
                new ApiScope("permissions"),

            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                 new Client
                 {
                     ClientId = "erp",
                     ClientSecrets = { new Secret("erp@cn".Sha256()) },
                     AllowedGrantTypes = new List<string>
                     {
                         GrantType.AuthorizationCode,
                         GrantType.ClientCredentials,
                         GrantType.ResourceOwnerPassword,
                         "dingtalk",
                         "dingtalk_scan"
                     },
                     AllowedScopes = new List<string>
                     {
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile,
                         IdentityServerConstants.StandardScopes.Email,
                         IdentityServerConstants.StandardScopes.Phone,
                         "roles",
                         "permissions"
                     },
                     RedirectUris = new List<string>
                     {
                         "https://erp.cn/oidc/passport/callback",
                         "https://erp.xyz/oidc/passport/callback",
                         "https://erp.top/oidc/passport/callback",
                         "https://51vip.biz/oidc/passport/callback"
                     },
                 },
                 // interactive ASP.NET Core MVC client
                 new Client
                 {
                     ClientId = "mvc",
                     ClientSecrets = { new Secret("secret".Sha256()) },
                     AllowedGrantTypes = GrantTypes.Code,
                 
                     // 登录后重定向到哪里
                     RedirectUris = { "https://localhost:5002/signin-oidc" },
                 
                     // 注销后重定向到哪里
                     PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                     AllowedScopes = new List<string>
                     {
                         IdentityServerConstants.StandardScopes.OpenId,
                         IdentityServerConstants.StandardScopes.Profile,
                         "erp"
                     }
                 }
            };
    }
}
