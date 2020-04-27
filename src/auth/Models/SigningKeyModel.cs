using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using Test.auth.Services;

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
        public T Future { get; set; }
    }

    public class SigningKeyModel<T>
        where T : AsymmetricSecurityKey
    {
        public SigningKeyModel(string name, string version)
        {
            Name = name;
            Version = version;
        }
        public string Name { get; set; }
        public string Version { get; set; }
        public string AlgorithmString { get; set; }
        public string Raw { get; set; }
        public T Key { get; set; }
        public string KeyType { get; set; }
        public string CurveName { get; set; }
        public string SignatureAlgorithm { get; set; }
        public DateTime? Activation { get; set; }
        public DateTime? Expiration { get; set; }

        public SecurityKeyInfo GetSecurityKeyInfo()
        {
            return new SecurityKeyInfo
            {
                Key = Key,
                SigningAlgorithm = AlgorithmString
            };
        }
    }
    public class RsaSigningKeyModel : SigningKeyModel<RsaSecurityKey>
    {
        public RsaSigningKeyModel(string name, string version) : base(name, version)
        { }
        public IdentityServerConstants.RsaSigningAlgorithm Algorithm { get; set; }
    }

    public class EcSigningKeyModel : SigningKeyModel<ECDsaSecurityKey>
    {
        public EcSigningKeyModel(string name, string version) : base(name, version)
        { }

        private IdentityServerConstants.ECDsaSigningAlgorithm _algorithm;
        public IdentityServerConstants.ECDsaSigningAlgorithm Algorithm { get
            {
                return _algorithm;
            }
            set
            {
                _algorithm = value;
                AlgorithmString = KeyCryptoHelper.GetECDsaSigningAlgorithmValue(value);
            }
        }
    }
}
