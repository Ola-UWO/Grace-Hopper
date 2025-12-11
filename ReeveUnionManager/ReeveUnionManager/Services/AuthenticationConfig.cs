namespace ReeveUnionManager.Services
{
    /// <summary>
    /// Configuration values used for Microsoft authentication (MSAL).
    /// These values must match the registered Azure AD / Entra application.
    /// </summary>
    public static class AuthenticationConfig
    {
        /// <summary>
        /// Client/Application ID from the app registration in Azure Entra.
        /// </summary>
        public const string ClientId = "e27b8799-3c66-4897-898d-9ab6b077791a";

        /// <summary>
        /// The app's tenant ID (not actively used when the authority
        /// allows "any organization + personal Microsoft accounts").
        /// Included for completeness and future flexibility.
        /// </summary>
        public const string TenantId = "bd679378-7260-4bb9-abe0-efd2763db1bb";

        /// <summary>
        /// OAuth scopes requested during sign-in.
        /// - Files.Read: required to read OneDrive folders/files
        /// - offline_access: required to enable silent token refresh
        /// - openid: identifies user and enables account selection
        /// </summary>
        public static readonly string[] Scopes =
        {
            "Files.ReadWrite.All",
            "offline_access",
            "openid"
        };

        /// <summary>
        /// Redirect URI used by MSAL during authentication.
        /// Must exactly match the URI defined in the Azure app registration.
        /// </summary>
        public static string RedirectUri => $"msal{ClientId}://auth";
    }
}
