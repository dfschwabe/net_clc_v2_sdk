using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Models;

namespace CenturyLinkCloudSdk.Services
{
    public interface ICenturyLinkCloudAntiAffinityPolicyService
    {
        Task<AntiAffinityPolicy> Get(string policyId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<AntiAffinityPolicy>> Get(CancellationToken cancellationToken = default(CancellationToken));
    }
}