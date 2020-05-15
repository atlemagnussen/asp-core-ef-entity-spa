using System;
using System.Net.Http;
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
                await TestAuthClients.Do();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
