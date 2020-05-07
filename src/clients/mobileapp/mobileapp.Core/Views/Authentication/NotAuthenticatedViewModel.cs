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
                await App.AuthService.LoginAsync();
            });
        }

        public ICommand AuthenticateCommand { get; }
    }
}
