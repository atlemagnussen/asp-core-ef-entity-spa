using System.Windows.Input;
using Xamarin.Forms;

namespace mobileapp.Core.ViewModels
{
    public class CustomerDetailViewModel : BaseViewModel
    {
        public Customer Customer { get; set; }

        public ICommand DeleteCommand { get; }

        public CustomerDetailViewModel(Customer customer = null)
        {
            Title = $"{customer?.FirstName} {customer?.LastName}";
            Customer = customer;
            DeleteCommand = new Command(() =>
            {
                MessagingCenter.Send(this, "DeleteItem", Customer);
            });
        }
    }
}
