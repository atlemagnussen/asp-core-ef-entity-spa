using IdentityServer4;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Test.auth.Models
{
    public class RsaSigningKeys : SigningKeys<RsaSigningKeyModel, RsaSecurityKey>
    {
    }

    public class EcSigningKeys : SigningKeys<EcSigningKeyModel, ECDsaSecurityKey>
    {
    }

    public class SigningKeys<T, TKey>
        where T : SigningKeyModel<TKey>
        where TKey : AsymmetricSecurityKey
    {
        public T Current { get; set; }
        public T Previous { get; set; }
    }

    public class SigningKeyModel<T>
        where T : AsymmetricSecurityKey
    {
        public string KeyId { get; set; }
        public string Raw { get; set; }
        public T Key { get; set; }
        public string KeyType { get; set; }
        public string CurveName { get; set; }
        public string SignatureAlgorithm { get; set; }
        public DateTime? Activation { get; set; }
        public DateTime? Expiration { get; set; }
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
