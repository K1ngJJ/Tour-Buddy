using Microsoft.Maui.Authentication;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TourBuddy.Services.Google
{
    public class GoogleAuthService
    {
        private readonly string _clientId = "140189883808-vok5rnoo7r2eq7m2k66n46jqjpg1uuta.apps.googleusercontent.com";
        private readonly string _redirectUri = "com.itdc.tourbuddy:/oauth2redirect";
        private readonly string[] _scopes = { "openid", "profile", "email" };

        public async Task<GoogleUser> AuthenticateAsync()
        {
            try
            {
                WebAuthenticatorResult authResult = await WebAuthenticator.AuthenticateAsync(
                    new WebAuthenticatorOptions
                    {
                        Url = new Uri($"https://accounts.google.com/o/oauth2/v2/auth" +
                                     $"?client_id={_clientId}" +
                                     $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}" +
                                     $"&response_type=code" +
                                     $"&scope={string.Join(" ", _scopes.Select(Uri.EscapeDataString))}" +
                                     $"&prompt=select_account"),
                        CallbackUrl = new Uri(_redirectUri)
                    });

                string authCode = authResult.Properties["code"];

                // Exchange the authorization code for tokens
                var tokens = await ExchangeCodeForTokensAsync(authCode);

                // Get user information using the access token
                var userInfo = await GetUserInfoAsync(tokens.AccessToken);

                return userInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication failed: {ex.Message}");
                throw;
            }
        }

        private async Task<TokenResponse> ExchangeCodeForTokensAsync(string code)
        {
            using var httpClient = new HttpClient();

            var tokenRequestParameters = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", _clientId },
                    { "redirect_uri", _redirectUri },
                    { "grant_type", "authorization_code" }
                };

            var tokenRequest = new FormUrlEncodedContent(tokenRequestParameters);
            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", tokenRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token exchange failed: {response.StatusCode}, Details: {errorContent}");
                throw new Exception($"Token exchange failed: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(responseJson);
        }


        private async Task<GoogleUser> GetUserInfoAsync(string accessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            response.EnsureSuccessStatusCode();

            var userInfoJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GoogleUser>(userInfoJson);
        }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
    }

    public class GoogleUser
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }
    }
}