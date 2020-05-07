using mobileapp.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace mobileapp.Core.Services
{
    public class CustomerDataStore : IDataStore<Customer>
    {
        private readonly Lazy<HttpClient> _apiClient;
        private static string _baseApiUrl;

        public CustomerDataStore()
        {
            _baseApiUrl = "https://asp-core-webapi.azurewebsites.net";
            _apiClient= new Lazy<HttpClient>(() => new HttpClient());
            _apiClient.Value.BaseAddress = new Uri(_baseApiUrl);

            var state = App.AuthService.GetCurrentState();
            if (state.LoggedIn)
                SetBearerToken(state.AccessToken);
        }

        public void SetBearerToken(string token)
        {
            _apiClient.Value.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public Task<bool> AddItemAsync(Customer item)
        {
            return Task.FromResult(true);
        }

        public Task<bool> UpdateItemAsync(Customer item)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            return Task.FromResult(true);
        }

        public Task<Customer> GetItemAsync(string id)
        {
            return Task.FromResult(new Customer
            {
                Id = 1,
                FirstName = "hello",
                LastName = "world"
            });
        }

        public async Task<IEnumerable<Customer>> GetItemsAsync(bool forceRefresh = false)
        {
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
