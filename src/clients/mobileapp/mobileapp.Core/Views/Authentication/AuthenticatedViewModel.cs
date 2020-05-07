using mobileapp.Core.ViewModels;
using System.Windows.Input;
using Xamarin.Forms;

namespace mobileapp.Core.Views.Authentication
{
    public class AuthenticatedViewModel : BaseViewModel
    {
        public AuthenticatedViewModel()
        {
            LogoutCommand = new Command(async () => 
            {
                await App.AuthService.LogoutAsync();
            });
            var state = App.AuthService.GetCurrentState();
            UserName = state.UserName;
            IdToken = state.IdentityToken;
            RefreshToken = state.RefreshToken;
        }

        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }

        public ICommand LogoutCommand { get; }
    }
}
