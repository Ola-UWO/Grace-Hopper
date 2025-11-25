using Microsoft.Identity.Client;
using System.Diagnostics;

namespace ReeveUnionManager.Services
{
    public static class AuthService
    {
        private static IPublicClientApplication _pca;
        private static AuthenticationResult? _lastResult;

        static AuthService()
        {
            _pca = PublicClientApplicationBuilder
                .Create(AuthenticationConfig.ClientId)
                // Allow ANY org + personal Microsoft accounts
                .WithAuthority(
                    AzureCloudInstance.AzurePublic,
                    AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
                .WithRedirectUri(AuthenticationConfig.RedirectUri)
                .Build();
        }

        /// <summary>
        /// Last acquired access token (null if not signed in).
        /// </summary>
        public static string? AccessToken => _lastResult?.AccessToken;

        /// <summary>
        /// Username / UPN for the signed-in account (for UI).
        /// </summary>
        public static string? SignedInUser =>
            _lastResult?.Account?.Username;

        /// <summary>
        /// Sign in the user.
        /// Tries silent login first, then falls back to interactive.
        /// </summary>
        public static async Task<AuthenticationResult?> SignInAsync()
        {
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
                // Silent failed, will go interactive
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AuthService] Silent sign-in failed: {ex}");
            }

            try
            {
                _lastResult = await _pca
                    .AcquireTokenInteractive(AuthenticationConfig.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();

                return _lastResult;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AuthService] Interactive sign-in failed: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Clear cached account info (does not revoke tokens server-side).
        /// </summary>
        public static async Task SignOutAsync()
        {
            if (_pca == null)
                return;

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
