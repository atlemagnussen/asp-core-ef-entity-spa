using Xamarin.Forms;
using mobileapp.Core.Services;
using mobileapp.Core.Views;

namespace mobileapp.Core
{
    public partial class App : Application
    {
        public static AuthService AuthService { get; set; }

        public App()
        {
            InitializeComponent();
            AuthService = new AuthService();
            DependencyService.Register<CustomerDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
