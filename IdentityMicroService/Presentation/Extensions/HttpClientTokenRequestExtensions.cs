using IdentityModel.Client;
using IdentityModel;

namespace IdentityMicroService.Presentation.Extensions
{
    public static class HttpClientTokenRequestExtensions
    {
        public static async Task<TokenResponse> RequestEmailPasswordTokenAsync(this HttpMessageInvoker client, EmailPasswordTokenRequest request, CancellationToken cancellationToken = default)
        {
            var clone = request.Clone<EmailPasswordTokenRequest>();

            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, request.GrantType);
            clone.Parameters.AddRequired(OidcConstants.StandardScopes.Email, request.Email);
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Password, request.Password, allowEmptyValue: true);
            clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

            foreach (var resource in request.Resource)
            {
                clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, allowDuplicates: true);
            }

            return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait(true);
        }
    }
}
