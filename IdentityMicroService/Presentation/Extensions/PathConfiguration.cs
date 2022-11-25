using Microsoft.AspNetCore.Mvc.Razor;
using System.Reflection;

namespace IdentityMicroService.Presentation.Extensions
{
    public static class PathConfiguration
    {
        public const string tokenRoute = "https://localhost:8080/connect/token";
        public const string cookieName = "IdentityServer.Cookies";
        public const string loginPath = "/Auth/Login";
        public const string resourceOwnerEmailPassword = "emailpassword";
        public const string clientId = "myClient";
        public const string testApiScope = "TestsAPI";
        public const string pathToView = "/Presentation/Views/{1}/{0}";
    }
}
