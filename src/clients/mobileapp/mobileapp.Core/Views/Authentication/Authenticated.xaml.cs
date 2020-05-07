using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mobileapp.Core.Views.Authentication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Authenticated : ContentView
    {
        public AuthenticatedViewModel viewModel;
        public Authenticated()
        {
            InitializeComponent();

            BindingContext = viewModel = new AuthenticatedViewModel();
        }
    }
}