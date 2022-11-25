using IdentityServer4;
using IdentityServer4.Models;
using System.Security.Claims;

namespace IdentityMicroService
{
    public static class Configuration
    {
        internal static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>
            {
                new ApiResource("TestsAPI")
            };

        internal static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope("TestsAPI")
            };

        internal static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client
                {
                    ClientId = "myClient",
                    ClientName = "My Custom Client",
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 60 * 60 * 24,
                    AllowedGrantTypes = { "emailpassword", GrantType.ResourceOwnerPassword},
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        "TestsAPI",
                        IdentityServerConstants.StandardScopes.OpenId,
                    },
                }
            };

        internal static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };
    }
}
