using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Extensions;
using Newtonsoft.Json;

namespace CenturyLinkCloudSdk.Runtime.Client
{
    public interface IHttpClient
    {
        Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken);
    }

    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _innerClient;

        public HttpClientWrapper(HttpClient innerClient)
        {
            _innerClient = innerClient;
        }

        public async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
        {
            var response = await _innerClient.GetAsync(requestUri, cancellationToken);
            
            response.EnsureCloudServiceSuccess();

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}