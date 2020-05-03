using IdentityServer4.Models;

namespace Test.auth.Models
{
    public class AdminViewModel
    {
        public Client WebClient { get; set; }
        public EcSigningKeys SigningKeys { get; set; }
        public string AuthConnectionString { get; set; }
        public string BankConnectionString { get; set; }
        public string AzureAdClientId { get; set; }
        public bool IsDevelopment { get; set; }
        public string ProtectInput { get; set; }
        public string ProtectedInput { get; set; }
        public string UnprotectedInput { get; set; }
    }
}
