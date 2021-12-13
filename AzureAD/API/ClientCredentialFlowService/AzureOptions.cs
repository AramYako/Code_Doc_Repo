using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCredentialFlowService
{
    public class AzureOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ResourceId { get; set; }
        public string Authority
        {
            get { return String.Format(CultureInfo.InvariantCulture, Instance, TenantId); }
        }
    }
}
