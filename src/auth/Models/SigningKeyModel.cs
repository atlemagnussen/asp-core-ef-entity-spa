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
        public SigningKeyModel(string name, string version, DateTimeOffset? notBefore, DateTimeOffset? expiresOn)
        {
            Name = name;
            Version = version;
            NotBefore = notBefore;
            ExpiresOn = expiresOn;
        }
        public string Name { get; set; }
        public string Version { get; set; }
        public string AlgorithmString { get; set; }
        public string Raw { get; set; }
        public T Key { get; set; }
        public string KeyType { get; set; }
        public string CurveName { get; set; }
        public string SignatureAlgorithm { get; set; }
        public DateTimeOffset? NotBefore { get; set; }
        public DateTimeOffset? ExpiresOn { get; set; }

        public override string ToString()
        {
            return $"{Version}-{Name}";
        }

        public bool Expired {
            get
            {
                if (!ExpiresOn.HasValue)
                    return false;
                if (ExpiresOn.Value <= DateTimeOffset.Now)
                    return true;
                return false;
            }
        }

        public bool Started
        {
            get
            {
                if (!NotBefore.HasValue)
                    return true;
                if (NotBefore.Value < DateTimeOffset.Now)
                    return true;
                return false;
            }
        }

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
        public RsaSigningKeyModel(string name, string version, DateTimeOffset? notBefore, DateTimeOffset? expiresOn) : base(name, version, notBefore, expiresOn)
        { }
        public IdentityServerConstants.RsaSigningAlgorithm Algorithm { get; set; }
    }

    public class EcSigningKeyModel : SigningKeyModel<ECDsaSecurityKey>
    {
        public EcSigningKeyModel(string name, string version, DateTimeOffset? notBefore, DateTimeOffset? expiresOn) : base(name, version, notBefore, expiresOn)
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
