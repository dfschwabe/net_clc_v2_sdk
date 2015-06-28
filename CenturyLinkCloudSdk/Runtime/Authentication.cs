using System.Collections.Generic;

namespace CenturyLinkCloudSdk.Runtime
{
    public class Authentication
    {
        public string BearerToken { get; set; }

        public string AccountAlias { get; set; }

        public string LocationAlias { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}