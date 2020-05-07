namespace mobileapp.Core.ViewModels
{
    public class AuthViewModel : BaseViewModel
    {
        private bool _isAuthenticated;

        public AuthViewModel()
        {
            IsAuthenticated = false;
        }
        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
            set
            {
                _isAuthenticated = value;
                OnPropertyChanged("IsAuthenticated");
            }
        }
        public bool IsNotAuthenticated
        {
            get
            {
                return !_isAuthenticated;
            }
        }

        public string IsAuthenticatedText
        {
            get { return _isAuthenticated.ToString(); }
        }
    }
}
