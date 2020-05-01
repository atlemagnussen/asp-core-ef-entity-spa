using IdentityServer4.Models;

namespace Test.auth.Models
{
    public class AdminViewModel
    {
        public Client WebClient { get; set; }
        public EcSigningKeys SigningKeys { get; set; }
        public string ConnectionString1 { get; set; }
        public string AzureAdClientId { get; set; }
        public bool IsDevelopment { get; set; }
    }
}
