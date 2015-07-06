using System.Collections.Generic;

namespace CenturyLinkCloudSdk.Models.Internal
{
    public class ActivityFilter
    {
        public IEnumerable<string> Accounts { get; set; }

        public int Limit { get; set; }
    }
}