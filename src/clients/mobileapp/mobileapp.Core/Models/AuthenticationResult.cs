using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using System;

namespace mobileapp.Core.Models
{
    public class AuthenticationResult
    {
        public AuthenticationResult()
        {
            LoggedIn = false;
            ErrorMessage = "Not logged in";
        }
        public AuthenticationResult(LoginResult result)
        {
            LoggedIn = true;
            AccessToken = result.AccessToken;
            IdentityToken = result.IdentityToken;
            RefreshToken = result.RefreshToken;
            AccessTokenExpiration = result.AccessTokenExpiration;
            AuthenticationTime = result.AuthenticationTime;
            //result.RefreshTokenHandler.InnerHandler.
        }

        public AuthenticationResult(string errorMessage)
        {
            LoggedIn = false;
            ErrorMessage = errorMessage;
        }

        public void Refresh(RefreshTokenResult refreshTokenResult)
        {
            LoggedIn = true;
            AccessToken = refreshTokenResult.AccessToken;
            AccessTokenExpiration = refreshTokenResult.AccessTokenExpiration;
            RefreshToken = refreshTokenResult.RefreshToken;
        }

        public bool LoggedIn { get; set; }
        public string ErrorMessage { get; set; }
        public string AccessToken { get; internal set; }
        public string IdentityToken { get; internal set; }
        public string RefreshToken { get; internal set; }
        public DateTime AccessTokenExpiration { get; internal set; }
        public DateTime AuthenticationTime { get; internal set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
    }
}
