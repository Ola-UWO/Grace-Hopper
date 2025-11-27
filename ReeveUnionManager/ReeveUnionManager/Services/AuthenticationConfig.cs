namespace ReeveUnionManager.Services
{
    public static class AuthenticationConfig
    {
        // From your app registration
        public const string ClientId = "e27b8799-3c66-4897-898d-9ab6b077791a";
        public const string TenantId = "bd679378-7260-4bb9-abe0-efd2763db1bb";

        // Scopes we requested in Entra
        public static readonly string[] Scopes =
        {
            "Files.Read",     // for OneDrive read
            "offline_access", // for refresh/silent
            "openid"          // basic OIDC
            // "User.Read" // optional, only if you added it
        };

        // This must match the redirect URI configured in Entra
        public static string RedirectUri => $"msal{ClientId}://auth";
    }
}
