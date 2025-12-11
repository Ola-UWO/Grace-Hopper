using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Maui.ApplicationModel; // Platform.CurrentActivity (Android)

namespace ReeveUnionManager.Services
{
    /// <summary>
    /// Wraps MSAL public client authentication for the app.
    /// Handles acquiring and caching access tokens for Microsoft Graph.
    /// </summary>
    public static class AuthService
    {
        private static readonly IPublicClientApplication _pca;
        private static AuthenticationResult? _lastResult;

        static AuthService()
        {
            var builder = PublicClientApplicationBuilder
                .Create(AuthenticationConfig.ClientId)
                // Allow any organization + personal Microsoft accounts
                .WithAuthority(
                    AzureCloudInstance.AzurePublic,
                    AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                .WithRedirectUri(AuthenticationConfig.RedirectUri);

            _pca = builder.Build();
        }

        /// <summary>
        /// Last acquired access token, or null if no user is currently signed in.
        /// </summary>
        public static string? AccessToken => _lastResult?.AccessToken;

        /// <summary>
        /// Username / UPN for the signed-in account (for display in the UI), or null if not signed in.
        /// </summary>
        public static string? SignedInUser => _lastResult?.Account?.Username;

        /// <summary>
        /// True if an access token has been acquired for the current session.
        /// This is a convenience wrapper around <see cref="AccessToken"/>.
        /// </summary>
        public static bool IsSignedIn => !string.IsNullOrWhiteSpace(AccessToken);

        /// <summary>
        /// Signs in the user.
        /// Attempts silent login first using any cached account, then falls back to interactive.
        /// Returns the <see cref="AuthenticationResult"/> or throws if sign-in fails.
        /// </summary>
        public static async Task<AuthenticationResult?> SignInAsync()
        {
            // 1) Try silent sign-in with any cached account
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                var firstAccount = accounts.FirstOrDefault();

                if (firstAccount != null)
                {
                    _lastResult = await _pca
                        .AcquireTokenSilent(AuthenticationConfig.Scopes, firstAccount)
                        .ExecuteAsync();

                    return _lastResult;
                }
            }
            catch (MsalUiRequiredException)
            {
                // Silent sign-in failed; will fall back to interactive below.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AuthService] Silent sign-in failed: {ex}");
            }

            // 2) Interactive sign-in — use system browser on ALL platforms
            var interactiveBuilder = _pca
                .AcquireTokenInteractive(AuthenticationConfig.Scopes)
                .WithPrompt(Prompt.SelectAccount);

#if ANDROID
            // Ensure MSAL knows which Activity initiated the interactive flow
            var activity = Platform.CurrentActivity;
            interactiveBuilder = interactiveBuilder.WithParentActivityOrWindow(() => activity);
#endif

            try
            {
                _lastResult = await interactiveBuilder.ExecuteAsync();
                return _lastResult;
            }
            catch (MsalException ex)
            {
                Debug.WriteLine($"[AuthService] Interactive MSAL sign-in failed: {ex}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AuthService] Interactive sign-in failed: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Signs out the current user from this client by clearing cached accounts and tokens.
        /// Does not revoke tokens server-side.
        /// </summary>
        public static async Task SignOutAsync()
        {
            try
            {
                var accounts = await _pca.GetAccountsAsync();
                foreach (var account in accounts)
                {
                    await _pca.RemoveAsync(account);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AuthService] Sign-out failed: {ex}");
            }

            _lastResult = null;
        }
    }
}
