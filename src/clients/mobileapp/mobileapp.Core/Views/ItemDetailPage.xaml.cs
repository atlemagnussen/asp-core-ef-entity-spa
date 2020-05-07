using mobileapp.Core.Models;
using mobileapp.Core.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace mobileapp.Core.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemDetailPage : ContentPage
    {
        CustomerDetailViewModel viewModel;

        public ItemDetailPage(CustomerDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var cus = new Customer
            {
                Id = 0,
                FirstName = "Test",
                LastName = "Testson"
            };

            viewModel = new CustomerDetailViewModel(cus);
            BindingContext = viewModel;
        }
    }
}