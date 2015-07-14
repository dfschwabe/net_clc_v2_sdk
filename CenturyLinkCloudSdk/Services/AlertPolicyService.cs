using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;

namespace CenturyLinkCloudSdk.Services
{
    public interface ICenturyLinkCloudAlertPolicyService
    {
         Task<AlertPolicy> Create(AlertPolicyDefniition definition, CancellationToken cancellationToken = default(CancellationToken));
    }
}