using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using mobileapp.Core.Views;

namespace mobileapp.Core.ViewModels
{
    public class CustomersViewModel : BaseViewModel
    {
        public ObservableCollection<Customer> Customers { get; set; }
        public Command LoadItemsCommand { get; set; }

        public CustomersViewModel()
        {
            Title = "Customers";
            Customers = new ObservableCollection<Customer>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Customer>(this, "AddItem", async (obj, cus) =>
            {
                var newCus = cus as Customer;
                Customers.Add(newCus);
                await App.DataStore.AddItemAsync(newCus);
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Customers.Clear();
                var items = await App.DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Customers.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}