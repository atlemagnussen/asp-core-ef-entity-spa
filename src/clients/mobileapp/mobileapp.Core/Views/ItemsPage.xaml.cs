using System;
using System.ComponentModel;
using Xamarin.Forms;
using mobileapp.Core.ViewModels;
using System.Linq;

namespace mobileapp.Core.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        CustomersViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new CustomersViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var customer = args.SelectedItem as Customer;
            if (customer == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new CustomerDetailViewModel(customer)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Customers.Count == 0 ||
                (viewModel.Customers.Count == 1 && viewModel.Customers.First().FirstName == "Un"))
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}