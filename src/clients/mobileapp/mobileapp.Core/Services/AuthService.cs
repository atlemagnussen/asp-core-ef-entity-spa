using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using mobileapp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace mobileapp.Core.Services
{
    public class AuthService
    {
        public OidcClient OidcClient { get; set; }
        public static AuthenticationResult State { get; internal set; }
        public SecretsStorage SecretsStorage { get; set; }

        public AuthService()
        {
            SecretsStorage = new SecretsStorage();
            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                Authority = "https://asp-core-auth-server.azurewebsites.net",
                //Authority = "https://10.0.2.2:6001", //debug
                //BackchannelHandler = DebugHttpHandler.GetInsecureHandler(), //debug
                ClientId = "mobileapp",
                Scope = "openid profile email roles api.read offline_access",
                RedirectUri = "com.companyname.mobileapp://callback",
                PostLogoutRedirectUri = "com.companyname.mobileapp://callback",
                
                Browser = browser,
                Policy = new Policy
                {
                    RequireIdentityTokenSignature = false
                }
            };

            OidcClient = new OidcClient(options);
        }

        public async Task<AuthenticationResult> GetCurrentState()
        {
            if (State == null)
                State = await SecretsStorage.GetStoredState();
            return State;
        }
        /// <summary>
        /// Login first time or when logged out
        /// </summary>
        public async Task<AuthenticationResult> LoginAsync()
        {
            await SecretsStorage.ClearAll();

            var result = await OidcClient.LoginAsync();

            // on successful login redirect you come here
            if (result.IsError)
            {
                return new AuthenticationResult(result.Error);
            }

            var res = new AuthenticationResult(result);

            var userName = ClaimsHelper.GetUserName(result.User.Claims);
            if (userName == null)
                res.ErrorMessage = "Could not get username from claims";
            res.UserName = userName;

            var fullName = ClaimsHelper.GetFullName(result.User.Claims);
            res.FullName = fullName;

            await SecretsStorage.StoreAccessToken(res.AccessToken);
            await SecretsStorage.StoreIdToken(res.IdentityToken);
            await SecretsStorage.StoreRefreshToken(res.RefreshToken);
            await SecretsStorage.StoreUserName(res.UserName);
            await SecretsStorage.StoreFullName(fullName);
            await SecretsStorage.StoreAccessTokenExpiration(res.AccessTokenExpiration);
            State = res;
            return res;
        }

        /// <summary>
        /// Check if current token is still valid
        /// </summary>
        public async Task<bool> CheckLoggedIn()
        {
            var accessToken = await SecretsStorage.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
                return false;

            UserInfoResult user;
            try
            {
                user = await OidcClient.GetUserInfoAsync(accessToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                user = null;
            }
            // isError means token expired
            if (user != null && !user.IsError)
                return true;

            return false;
        }

        /// <summary>
        /// Tries to refresh token if there is refresh_token
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryRefreshToken()
        {
            var refresh_token = await SecretsStorage.GetRefreshToken();
            if (string.IsNullOrEmpty(refresh_token))
                return false;

            var res = await OidcClient.RefreshTokenAsync(refresh_token);
            if (res.IsError)
            {
                //ErrorMessage = result.Error;
                return false;
            }
            State.Refresh(res);
            await SecretsStorage.StoreAccessToken(res.AccessToken);
            await SecretsStorage.StoreIdToken(res.IdentityToken);
            await SecretsStorage.StoreRefreshToken(res.RefreshToken);
            return true;
        }

        /// <summary>
        /// Log out
        /// </summary>
        public async Task<LogoutResult> LogoutAsync()
        {
            var request = new LogoutRequest();
            request.IdTokenHint = await SecretsStorage.GetIdToken();
            var result = await OidcClient.LogoutAsync(request);

            if (!result.IsError)
            {
                State = new AuthenticationResult();
                await SecretsStorage.ClearAll();
            }
            return result;
        }

        private Claim TryClaims(IEnumerable<Claim> claims, IEnumerable<string> tryClaimNames)
        {
            foreach (var claimName in tryClaimNames)
            {
                var claim = claims.FirstOrDefault(x => x.Type == claimName);
                if (claim != null)
                    return claim;
            }
            return null;
        }

        
    }
}
