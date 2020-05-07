using mobileapp.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace mobileapp.Core.Services
{
    public class CustomerDataStore : IDataStore<Customer>
    {
        private readonly Lazy<HttpClient> _apiClient;
        private static string _baseApiUrl;
        private string accessToken = string.Empty;

        public CustomerDataStore()
        {
            _baseApiUrl = "https://asp-core-webapi.azurewebsites.net";
            _apiClient= new Lazy<HttpClient>(() => new HttpClient());
            _apiClient.Value.BaseAddress = new Uri(_baseApiUrl);
        }

        public void SetBearerToken(string token)
        {
            accessToken = token;
            _apiClient.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task TrySetBearerToken()
        {
            var state = await App.AuthService.GetCurrentState();
            if (state.LoggedIn && state.AccessToken != accessToken)
                SetBearerToken(state.AccessToken);
        }
        public async Task<bool> AddItemAsync(Customer item)
        {
            await TrySetBearerToken();
            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            var result = await _apiClient.Value.PostAsync("/api/customers", customerInfo);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(Customer item)
        {
            await TrySetBearerToken();
            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");

            var result = await _apiClient.Value.PutAsync("/api/customers", customerInfo);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            await TrySetBearerToken();
            var result = await _apiClient.Value.DeleteAsync($"/api/customers/{id}");
            return result.IsSuccessStatusCode;
        }

        public async Task<Customer> GetItemAsync(string id)
        {
            await TrySetBearerToken();
            var result = await _apiClient.Value.GetAsync($"/api/customers/{id}");

            if (result.IsSuccessStatusCode)
            {
                var res = await result.Content.ReadAsStringAsync();
                var resJson = JsonConvert.DeserializeObject<Customer>(res);

                return resJson;
            }
            else
            {
                return new Customer
                {
                    Id = 0,
                    FirstName = "Un",
                    LastName = "Authorized"
                };
            }
        }

        public async Task<IEnumerable<Customer>> GetItemsAsync(bool forceRefresh = false)
        {
            await TrySetBearerToken();
            var result = await _apiClient.Value.GetAsync("/api/customers");

            if (result.IsSuccessStatusCode)
            {
                var res = await result.Content.ReadAsStringAsync();
                var resJson = JsonConvert.DeserializeObject<IEnumerable<Customer>>(res);

                return resJson;
            }
            else
            {
                //throw new ApplicationException(result.ReasonPhrase);
                return new List<Customer>()
                {
                    new Customer
                    {
                        Id = 0,
                        FirstName = "Un",
                        LastName = "Authorized"
                    }
                };
            }
        }
    }
}
