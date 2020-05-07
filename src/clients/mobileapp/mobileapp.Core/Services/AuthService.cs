using IdentityModel;
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
        public AuthenticationResult State { get; internal set; }

        public AuthService()
        {
            State = new AuthenticationResult();
            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                Authority = "https://asp-core-auth-server.azurewebsites.net",
                //Authority = "https://10.0.2.2:6001", //debug
                ClientId = "mobileapp",
                Scope = "openid profile email roles api.read offline_access",
                RedirectUri = "com.companyname.mobileapp://callback",
                PostLogoutRedirectUri = "com.companyname.mobileapp://callback",
                
                Browser = browser,
                Policy = new Policy
                {
                    RequireIdentityTokenSignature = false
                },
                //BackchannelHandler = DebugHandler.GetInsecureHandler(), //debug
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };

            OidcClient = new OidcClient(options);
        }

        /// <summary>
        /// Login first time or when logged out
        /// </summary>
        public async Task<AuthenticationResult> LoginAsync()
        {
            if (Application.Current.Properties.ContainsKey("token"))
            {
                Application.Current.Properties.Remove("token");
                await Application.Current.SavePropertiesAsync();
            }

            var result = await OidcClient.LoginAsync();

            // on successful login redirect you come here
            if (result.IsError)
            {
                return new AuthenticationResult(result.Error);
            }

            var res = new AuthenticationResult(result);

            //var allClaims = result.User.Claims.ToArray();
            var userNameClaim = TryClaims(result.User.Claims, new string[] { JwtClaimTypes.PreferredUserName, JwtClaimTypes.Name, JwtClaimTypes.Email });

            if (userNameClaim == null)
            {
                res.ErrorMessage = "Could not get username from claims";
            }

            res.UserName = userNameClaim.Value;
            State = res;
            return res;
        }

        /// <summary>
        /// Check if current token is still valid
        /// </summary>
        public async Task<bool> CheckLoggedIn()
        {
            if (!App.Current.Properties.ContainsKey("token"))
                return false;
            var token = App.Current.Properties["token"].ToString();
            if (string.IsNullOrEmpty(token))
                return false;

            UserInfoResult user;
            try
            {
                user = await OidcClient.GetUserInfoAsync(token);
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
            string refresh_token = string.Empty;
            if (App.Current.Properties.ContainsKey("refresh_token"))
                refresh_token = App.Current.Properties["refresh_token"].ToString();

            if (string.IsNullOrEmpty(refresh_token))
                return false;

            var result = await OidcClient.RefreshTokenAsync(refresh_token);
            if (result.IsError)
            {
                //ErrorMessage = result.Error;
                return false;
            }
            State.Refresh(result);
            return true;
        }

        /// <summary>
        /// Log out
        /// </summary>
        public async Task<LogoutResult> LogoutAsync()
        {
            var result = await OidcClient.LogoutAsync();
            if (!result.IsError)
            {
                State = new AuthenticationResult();
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
