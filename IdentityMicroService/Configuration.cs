using IdentityMicroService.Presentation.Extensions;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityMicroService;

public static class Configuration
{
    internal static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new()
            {
                Name = PathConfiguration.TestApiScope,
                DisplayName = PathConfiguration.TestApiScope,
                Scopes = new List<string>
                {
                    PathConfiguration.TestApiScope
                }
            }
        };
    }

    internal static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new(PathConfiguration.TestApiScope)
        };
    }

    internal static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new()
            {
                ClientId = PathConfiguration.ClientId,
                AllowAccessTokensViaBrowser = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedGrantTypes = { PathConfiguration.ResourceOwnerEmailPassword, GrantType.ResourceOwnerPassword },
                RequireClientSecret = false,
                AllowOfflineAccess = true,
                AllowedScopes =
                {
                    PathConfiguration.TestApiScope,
                    IdentityServerConstants.StandardScopes.OpenId
                },
                AccessTokenLifetime = 36000
            }
        };
    }

    internal static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };
    }
}