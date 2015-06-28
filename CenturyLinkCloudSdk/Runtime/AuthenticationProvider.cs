using System.Threading.Tasks;

namespace CenturyLinkCloudSdk.Runtime
{
    public interface IAliasProvider
    {
        Task<string> GetAccountAlias();
    }
}