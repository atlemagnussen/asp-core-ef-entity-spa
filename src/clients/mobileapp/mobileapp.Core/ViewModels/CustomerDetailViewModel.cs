namespace mobileapp.Core.ViewModels
{
    public class CustomerDetailViewModel : BaseViewModel
    {
        public Customer Customer { get; set; }
        public CustomerDetailViewModel(Customer customer = null)
        {
            Title = $"{customer?.FirstName} {customer?.LastName}";
            Customer = customer;
        }
    }
}
