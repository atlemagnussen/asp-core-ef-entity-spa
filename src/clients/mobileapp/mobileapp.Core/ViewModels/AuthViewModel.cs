namespace mobileapp.Core.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        public AuthViewModel()
        {
            var currentState = App.AuthService.GetCurrentState();
            IsLoggedIn = currentState.LoggedIn;
        }

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
