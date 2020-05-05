using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Test.auth.Models
{
    public class SigningKeys
    {
        public SigningKeyModel Current { get; set; }
        public SigningKeyModel Previous { get; set; }
        public SigningKeyModel Future { get; set; }
        public DateTimeOffset? CacheExpiring { get; set; }
    }

    public class SigningKeyModel
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
        public AsymmetricSecurityKey Key { get; set; }
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
}
