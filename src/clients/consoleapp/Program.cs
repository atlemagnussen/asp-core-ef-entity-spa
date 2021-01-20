using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Test.consoleapp
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            try
            {
                //var link = "59adc752-ddac-42f5-8785-ce4d3cb62e3b";
                //var user = "3cfb0ad7-b98e-4032-9f27-1e4f894a655f";
                //var encodedLink = WebUtility.UrlEncode(link);
                //var encodedUser = WebUtility.UrlEncode(user);
                //Console.WriteLine(encodedLink);
                //Console.WriteLine(encodedUser);

                var g = Guid.NewGuid();
                var gb = GuidByteBase64(g);

                Console.WriteLine($"guid:{g}{Environment.NewLine}guid:{gb}");

                //await TestAuthClients.Do();
                var token = "bnEWxiqg2EhuMYDw86DYSOSMSt4fl3VHkzuylcZ0JiwzY2ZiMGFkNy1iOThlLTQwMzItOWYyNy0xZTRmODk0YTY1NWY=";
                var (created, expires, guid, user) = DecodeToken(token);
                Console.WriteLine($"created={created}");
                Console.WriteLine($"expires={expires}");
                Console.WriteLine($"guid={guid}");
                Console.WriteLine($"user={user}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static string GuidByteBase64(Guid guid)
        {
            byte[] key = guid.ToByteArray();
            return Convert.ToBase64String(key);
        }
        private static void UnitTestTokens()
        {
            var created = DateTime.UtcNow;
            var expires = created.AddDays(1);
            var guid = Guid.NewGuid();
            var userid = "3cfb0ad7-b98e-4032-9f27-1e4f894a655f";
            var token = GenerateToken(created, expires, guid, userid);
            var (createdDek, expiresDek, guidDek, useridDek) = DecodeToken(token);

            var createdSame = created == createdDek;
            Console.WriteLine($"{created}=={createdDek} ? {createdSame}");

            var expiresSame = expires == expiresDek;
            Console.WriteLine($"{expires}=={expiresDek} ? {expiresSame}");

            var guidSame = guid == guidDek;
            Console.WriteLine($"{guid}=={guidDek} ? {guidSame}");

            var userSame = userid == useridDek;
            Console.WriteLine($"{userid}=={useridDek} ? {userSame}");

            Console.WriteLine($"token={token}");
        }

        private static string GenerateToken(DateTime created, DateTime expires, Guid guid, string userid)
        {
            byte[] dateCreatedStamp = BitConverter.GetBytes(created.ToBinary());
            byte[] dateExpiryStamp = BitConverter.GetBytes(expires.ToBinary());
            byte[] key = guid.ToByteArray();
            byte[] user = Encoding.ASCII.GetBytes(userid);
            string linkId = Convert.ToBase64String(dateCreatedStamp.Concat(dateExpiryStamp).Concat(key).Concat(user).ToArray());
            return linkId;
        }

        private static (DateTime created, DateTime expired, Guid guid, string userid) DecodeToken(string token)
        {
            byte[] data = Convert.FromBase64String(token);
            DateTime created = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            DateTime expire = DateTime.FromBinary(BitConverter.ToInt64(data, 8));
            var guidByte = data.Skip(16).Take(16).ToArray();
            var guid = new Guid(guidByte);

            var userByte = data.Skip(32).ToArray();
            var user = Encoding.ASCII.GetString(userByte);
            return (created, expire, guid, user);
        }

        private static string CalculateNonce()
        {
            //Allocate a buffer
            var ByteArray = new byte[20];
            //Generate a cryptographically random set of bytes
            using (var Rnd = RandomNumberGenerator.Create())
            {
                Rnd.GetBytes(ByteArray);
            }
            //Base64 encode and then return
            return Convert.ToBase64String(ByteArray);
        }
    }
}
