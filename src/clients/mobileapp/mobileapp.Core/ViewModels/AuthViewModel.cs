﻿using System.Windows.Input;
using Xamarin.Forms;

namespace mobileapp.Core.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        public AuthViewModel()
        {
            LogoutCommand = new Command(async () =>
            {
                var result = await App.AuthService.LogoutAsync();
                IsLoggedIn = false;
            });
            AuthenticateCommand = new Command(async () =>
            {
                var result = await App.AuthService.LoginAsync();
                IsLoggedIn = result.LoggedIn;
                //MessagingCenter.Send(this, "AuthenticationUpdated", result);
                App.DataStore.SetBearerToken(result.AccessToken);
            });
        }

        public void Refresh()
        {
            var currentState = App.AuthService.GetCurrentState();
            IsLoggedIn = currentState.LoggedIn;
            UserName = currentState.UserName;
            AccessToken = currentState.AccessToken;
            IdToken = currentState.IdentityToken;
            RefreshToken = currentState.RefreshToken;
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName, value); }
        }

        private string accessToken;
        public string AccessToken
        {
            get { return accessToken; }
            set { SetProperty(ref accessToken, value); }
        }

        private string idToken;
        public string IdToken
        {
            get { return idToken; }
            set { SetProperty(ref idToken, value); }
        }

        private string refreshToken;
        public string RefreshToken
        {
            get { return refreshToken; }
            set { SetProperty(ref refreshToken, value); }
        }
        

        public ICommand AuthenticateCommand { get; }
        public ICommand LogoutCommand { get; }

        public bool NotLoggedIn
        {
            get
            {
                return !IsLoggedIn;
            }
        }

        public string IsAuthenticatedText
        {
            get { return IsLoggedIn.ToString(); }
        }
    }
}
