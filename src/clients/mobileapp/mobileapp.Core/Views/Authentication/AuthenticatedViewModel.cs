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
            UserName = App.AuthService.State.UserName;
        }

        public string UserName { get; set; }

        public ICommand LogoutCommand { get; }
    }
}
