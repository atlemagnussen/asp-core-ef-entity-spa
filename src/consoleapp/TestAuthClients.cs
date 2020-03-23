using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test.consoleapp
{
    class TestAuthClients
    {
        public static async Task Do()
        {
            // discover all the endpoints using metadata of identity server
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:6001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //var tokenResponse = await GetTokenClientCred(client, disco);
            var tokenResponse = await GetTokenResourceOwner(client, disco, "atlemagnussen@gmail.com", "Einherjer57!");

            client.SetBearerToken(tokenResponse.AccessToken);

            //await CreateCustomer(client, "From", "Console");
            await ListAll(client);
        }

        private static async Task ListAll(HttpClient client)
        {
            var customersResponse = await client.GetAsync("https://localhost:5001/api/customers");

            if (!customersResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"statusCode={customersResponse.StatusCode}");
                Console.WriteLine($"reason={customersResponse.ReasonPhrase}");
            }
            else
            {
                Console.WriteLine($"Request OK, statusCode={customersResponse.StatusCode}");
                var content = await customersResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }

        private static async Task CreateCustomer(HttpClient client, string first, string last)
        {
            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(
                    new { FirstName = first, LastName = last }), Encoding.UTF8, "application/json");

            var createCustomerResponse = await client.PostAsync("https://localhost:5001/api/customers", customerInfo);

            if (!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"statusCode={createCustomerResponse.StatusCode}");
                Console.WriteLine($"reason={createCustomerResponse.ReasonPhrase}");
            }
            else
            {
                Console.WriteLine(createCustomerResponse.StatusCode);
                var content = await createCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JObject.Parse(content));
            }
        }

        private static async Task<TokenResponse> GetTokenResourceOwner(HttpClient client, DiscoveryDocumentResponse disco, string user, string pass)
        {
            // Grab a bearer token
            var tokenOptions = new TokenClientOptions
            {
                Address = disco.TokenEndpoint,
                ClientId = "ro.client",
                ClientSecret = "secret"
            };
            var tokenClient = new TokenClient(client, tokenOptions);
            var tokenResponse = await tokenClient.RequestPasswordTokenAsync(user, pass, "bankApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine("Got token:");
            Console.WriteLine(tokenResponse.Json);
            return tokenResponse;
        }

        private static async Task<TokenResponse> GetTokenClientCred(HttpClient client, DiscoveryDocumentResponse disco)
        {
            // Grab a bearer token
            var tokenOptions = new TokenClientOptions
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret"
            };
            var tokenClient = new TokenClient(client, tokenOptions);
            var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync("bankApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }

            Console.WriteLine("Got token:");
            Console.WriteLine(tokenResponse.Json);
            return tokenResponse;
        }
    }
}
