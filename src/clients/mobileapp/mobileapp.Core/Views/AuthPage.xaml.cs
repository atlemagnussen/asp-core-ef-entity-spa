using mobileapp.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobileapp.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuthPage : ContentPage
    {
        public AuthViewModel viewModel;

        public AuthPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new AuthViewModel();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.Refresh();
        }
    }
}