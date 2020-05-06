using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Test.auth.Services
{
    public static class KeyCryptoHelper
    {
        internal static IdentityServerConstants.ECDsaSigningAlgorithm GetEcAlgorithm(KeyCurveName? keyCurveName)
        {
            if (!keyCurveName.HasValue)
                throw new NotSupportedException();

            if (keyCurveName.Value == KeyCurveName.P256)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES256;

            if (keyCurveName.Value == KeyCurveName.P384)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES384;

            if (keyCurveName.Value == KeyCurveName.P521)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES512;

            throw new NotSupportedException();
        }
        internal static string GetECDsaSigningAlgorithmValue(IdentityServerConstants.ECDsaSigningAlgorithm value)
        {
            return value switch
            {
                IdentityServerConstants.ECDsaSigningAlgorithm.ES256 => SecurityAlgorithms.EcdsaSha256,
                IdentityServerConstants.ECDsaSigningAlgorithm.ES384 => SecurityAlgorithms.EcdsaSha384,
                IdentityServerConstants.ECDsaSigningAlgorithm.ES512 => SecurityAlgorithms.EcdsaSha512,
                _ => throw new ArgumentException("Invalid ECDsa signing algorithm value", nameof(value)),
            };
        }
    }
}
