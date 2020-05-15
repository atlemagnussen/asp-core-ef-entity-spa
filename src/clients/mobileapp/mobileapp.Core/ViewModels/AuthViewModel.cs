using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public async Task Refresh()
        {
            var currentState = await App.AuthService.GetCurrentState();
            IsLoggedIn = currentState.LoggedIn;
            UserName = currentState.UserName;
            AccessToken = currentState.AccessToken;
            IdToken = currentState.IdentityToken;
            RefreshToken = currentState.RefreshToken;
            AccessTokenExpiration = currentState.AccessTokenExpiration.ToString(CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
            TimeNow = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
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

        private string accessTokenExpiration;
        public string AccessTokenExpiration
        {
            get { return accessTokenExpiration; }
            set { SetProperty(ref accessTokenExpiration, value); }
        }

        private string timeNow;
        public string TimeNow
        {
            get { return timeNow; }
            set { SetProperty(ref timeNow, value); }
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
