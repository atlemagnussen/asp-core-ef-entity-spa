using mobileapp.Core.ViewModels;
using System.Windows.Input;
using Xamarin.Forms;

namespace mobileapp.Core.Views.Authentication
{
    public class NotAuthenticatedViewModel : BaseViewModel
    {
        public NotAuthenticatedViewModel()
        {
            AuthenticateCommand = new Command(async () =>
            {
                var result = await App.AuthService.LoginAsync();
                IsLoggedIn = result.LoggedIn;
                App.DataStore.SetBearerToken(result.AccessToken);
            });
        }

        public ICommand AuthenticateCommand { get; }
    }
}
