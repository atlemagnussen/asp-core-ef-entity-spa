using IdentityServer4;
using Microsoft.IdentityModel.Tokens;

namespace Test.auth.Models
{
    public class SigningKeyModel
    {
        public string Raw { get; set; }
        public SecurityKey Key { get; set; }
        public string Algorithm { get; set; }
    }
    public class RsaSigningKeyModel
    {
        public string Raw { get; set; }
        public RsaSecurityKey Key { get; set; }
        public IdentityServerConstants.RsaSigningAlgorithm Algorithm { get; set; }
        public string AlgorithmString { get; set; }
    }

    public class EcSigningKeyModel
    {
        public string Raw { get; set; }
        public ECDsaSecurityKey Key { get; set; }
        public IdentityServerConstants.ECDsaSigningAlgorithm Algorithm { get; set; }
        public string AlgorithmString { get; set; }
    }
}
