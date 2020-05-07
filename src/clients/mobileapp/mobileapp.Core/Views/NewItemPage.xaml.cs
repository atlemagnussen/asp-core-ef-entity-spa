using mobileapp.Core.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace mobileapp.Core.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NewItemPage : ContentPage
    {
        public Customer Customer { get; set; }

        public NewItemPage()
        {
            InitializeComponent();

            Customer = new Customer
            {
                FirstName = "First Name",
                LastName = "Last Name"
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", Customer);
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}