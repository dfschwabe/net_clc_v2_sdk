using System.Net.Http;

namespace CenturyLinkCloudSdk.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static void EnsureCloudServiceSuccess(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new CloudServiceException(response);
            }
        }
    }
}