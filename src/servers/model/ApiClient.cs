namespace Test.model
{
    public class ApiClient
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public ClientSecretType SecretType { get; set; }
        public string Secret { get; set; }
    }
    public enum ClientSecretType
    {
        SharedSecret,
        JsonWebKey
    }
}
