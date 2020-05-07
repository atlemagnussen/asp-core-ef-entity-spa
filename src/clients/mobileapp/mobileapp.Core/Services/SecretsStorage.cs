using mobileapp.Core.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mobileapp.Core.Services
{
    public class SecretsStorage
    {
        private static string AccessToken = "access_token";
        private static string IdToken = "id_token";
        private static string RefreshToken = "refresh_token";
        private static string FullName = "full_name";
        private static string UserName = "user_name";
        private static string UserId = "user_id";
        private static string AccessTokenExpiration = "access_token_expiration";
        private static string Prefix = "mobileapp";

        /// <summary>
        /// storers
        /// </summary>
        internal async Task StoreAccessToken(string token)
        {
            await StoreSecretValue(AccessToken, token);
        }
        internal async Task StoreIdToken(string token)
        {
            await StoreSecretValue(IdToken, token);
        }
        internal async Task StoreRefreshToken(string token)
        {
            await StoreSecretValue(RefreshToken, token);
        }
        internal async Task StoreUserName(string name)
        {
            await StoreSecretValue(UserName, name);
        }
        internal async Task StoreFullName(string name)
        {
            await StoreSecretValue(FullName, name);
        }
        internal async Task StoreUserId(string id)
        {
            await StoreSecretValue(UserId, id);
        }
        internal async Task StoreAccessTokenExpiration(DateTime accessTokenExpiration)
        {
            string dateSer = JsonSerializer.Serialize(accessTokenExpiration);
            await StoreSecretValue(AccessTokenExpiration, dateSer);
        }
        /// <summary>
        /// retrievers
        /// </summary>

        internal async Task<AuthenticationResult> GetStoredState()
        {
            var accessToken = await GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
                return new AuthenticationResult();

            var state = new AuthenticationResult
            {
                AccessToken = accessToken,
                LoggedIn = true
            };
            state.IdentityToken = await GetIdToken();
            state.RefreshToken = await GetRefreshToken();
            state.UserName = await GetUserName();
            state.FullName = await GetFullName();
            state.UserId = await GetUserId();
            state.AccessTokenExpiration = await GetAccessTokenExpiration();
            return state;
        }
        internal async Task<string> GetAccessToken()
        {
            return await RetrieveSecret(AccessToken);
        }
        internal async Task<string> GetIdToken()
        {
            return await RetrieveSecret(IdToken);
        }
        internal async Task<string> GetRefreshToken()
        {
            return await RetrieveSecret(RefreshToken);
        }
        internal async Task<string> GetUserName()
        {
            return await RetrieveSecret(UserName);
        }
        internal async Task<string> GetFullName()
        {
            return await RetrieveSecret(FullName);
        }
        internal async Task<string> GetUserId()
        {
            return await RetrieveSecret(UserId);
        }
        internal async Task<DateTime> GetAccessTokenExpiration()
        {
            var dateSer = await RetrieveSecret(AccessTokenExpiration);
            return JsonSerializer.Deserialize<DateTime>(dateSer);
        }
        /// <summary>
        /// deleters
        /// </summary>

        internal async Task ClearAll()
        {
            await DeleteAccessToken();
            await DeleteIdToken();
            await DeleteRefreshToken();
            await DeleteUserId();
            await DeleteFullName();
            await DeleteUserId();
        }
        internal async Task DeleteAccessToken()
        {
            await DeleteSecret(AccessToken);
        }
        internal async Task DeleteIdToken()
        {
            await DeleteSecret(IdToken);
        }
        internal async Task DeleteRefreshToken()
        {
            await DeleteSecret(RefreshToken);
        }
        internal async Task DeleteUserName()
        {
            await DeleteSecret(UserName);
        }
        internal async Task DeleteFullName()
        {
            await DeleteSecret(FullName);
        }
        internal async Task DeleteUserId()
        {
            await DeleteSecret(UserId);
        }

        /// <summary>
        /// mechanisms
        /// </summary>
        private async Task StoreSecretValue(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("No name to store upon!");

            if (string.IsNullOrEmpty(value))
                return;

            string prefixedName = $"{Prefix}{name}";
            try
            {
                await SecureStorage.SetAsync(prefixedName, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Application.Current.Properties[prefixedName] = value;
                await Application.Current.SavePropertiesAsync();
            }
        }

        private async Task<string> RetrieveSecret(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("No name to store upon!");

            string prefixedName = $"{Prefix}{name}";
            try
            {
                return await SecureStorage.GetAsync(prefixedName);
            }
            catch (Exception)
            {
                if (Application.Current.Properties.ContainsKey(prefixedName))
                    return Application.Current.Properties[prefixedName].ToString();
            }
            return string.Empty;
        }

        private async Task DeleteSecret(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ApplicationException("No name to store upon!");

            string prefixedName = $"{Prefix}{name}";
            try
            {
                SecureStorage.Remove(prefixedName);
            }
            catch(Exception)
            {
                if (Application.Current.Properties.ContainsKey(prefixedName))
                {
                    Application.Current.Properties.Remove(prefixedName);
                    await Application.Current.SavePropertiesAsync();
                }
            }
        }
    }
}
