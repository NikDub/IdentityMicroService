using IdentityMicroService.Presentation.Extensions;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityMicroService
{
    public static class Configuration
    {
        internal static IEnumerable<ApiResource> GetApiResources() =>
            new List<ApiResource>
            {
                new ApiResource(PathConfiguration.testApiScope)
            };

        internal static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope(PathConfiguration.testApiScope)
            };

        internal static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client
                {
                    ClientId = PathConfiguration.clientId,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AccessTokenLifetime = 60 * 60 * 24,
                    AllowedGrantTypes = { PathConfiguration.resourceOwnerEmailPassword, GrantType.ResourceOwnerPassword},
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        PathConfiguration.testApiScope,
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
