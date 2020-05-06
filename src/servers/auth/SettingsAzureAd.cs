using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.auth
{
    public class SettingsAzureAd
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
    }
}
