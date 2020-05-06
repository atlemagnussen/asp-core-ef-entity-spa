using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using System;

namespace mobileapp.Core.Models
{
    public class AuthenticationResult
    {
        public AuthenticationResult(LoginResult result)
        {
            Success = true;
            AccessToken = result.AccessToken;
            IdentityToken = result.IdentityToken;
            RefreshToken = result.RefreshToken;
            AccessTokenExpiration = result.AccessTokenExpiration;
            AuthenticationTime = result.AuthenticationTime;
            //result.RefreshTokenHandler.InnerHandler.
        }

        public AuthenticationResult(RefreshTokenResult refreshTokenResult)
        {
            Success = true;
            AccessToken = refreshTokenResult.AccessToken;
            AccessTokenExpiration = refreshTokenResult.AccessTokenExpiration;
            RefreshToken = refreshTokenResult.RefreshToken;
        }
        public AuthenticationResult(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string AccessToken { get; internal set; }
        public string IdentityToken { get; internal set; }
        public string RefreshToken { get; internal set; }
        public DateTime AccessTokenExpiration { get; internal set; }
        public DateTime AuthenticationTime { get; internal set; }
        public string UserName { get; set; }
    }
}
