using IdentityModel.Client;

namespace IdentityMicroService.Presentation.Extensions;

/// <summary>
///     Request for token using password
/// </summary>
/// <seealso cref="TokenRequest" />
public class EmailPasswordTokenRequest : TokenRequest
{
    /// <summary>
    ///     Gets or sets the email of the user.
    /// </summary>
    /// <value>
    ///     The email of the user.
    /// </value>
    public string Email { get; set; }

    /// <summary>
    ///     Gets or sets the password.
    /// </summary>
    /// <value>
    ///     The password.
    /// </value>
    public string Password { get; set; }

    /// <summary>
    ///     Space separated list of the requested scopes
    /// </summary>
    /// <value>
    ///     The scope.
    /// </value>
    public string Scope { get; set; }

    /// <summary>
    ///     List of requested resources
    /// </summary>
    /// <value>
    ///     The scope.
    /// </value>
    public ICollection<string> Resource { get; set; } = new HashSet<string>();
}