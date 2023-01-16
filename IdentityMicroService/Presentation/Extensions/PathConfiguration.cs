namespace IdentityMicroService.Presentation.Extensions;

public static class PathConfiguration
{
    public const string TokenRoute = "/connect/token";
    public const string CookieName = "IdentityServer.Cookies";
    public const string LoginPath = "/Auth/Login";
    public const string PathToView = "/Presentation/Views/{1}/{0}";

    public const string ResourceOwnerEmailPassword = "emailpassword";
    public const string ClientId = "myClient";
    public const string TestApiScope = "TestsAPI";
    public const string Https = "https";
}