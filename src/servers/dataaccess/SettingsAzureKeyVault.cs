namespace Test.dataaccess
{
    public class SettingsAzureKeyVault
    {
        public string VaultName { get; set; }
        public string SigningKeyName { get; set; }
        public string DataProtectionKeyName { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public string ClientSecret { get; set; }
    }
}
