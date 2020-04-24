using IdentityServer4;
using Microsoft.IdentityModel.Tokens;

namespace Test.auth.Models
{
    public class SigningKeyModel<T>
        where T : SecurityKey
    {
        public string KeyId { get; set; }
        public string Raw { get; set; }
        public T Key { get; set; }
        public string KeyType { get; set; }
        public string CurveName { get; set; }
        public string SignatureAlgorithm { get; set; }
    }
    public class RsaSigningKeyModel : SigningKeyModel<RsaSecurityKey>
    {
        public IdentityServerConstants.RsaSigningAlgorithm Algorithm { get; set; }
    }

    public class EcSigningKeyModel : SigningKeyModel<ECDsaSecurityKey>
    {
        public IdentityServerConstants.ECDsaSigningAlgorithm Algorithm { get; set; }
    }
}
