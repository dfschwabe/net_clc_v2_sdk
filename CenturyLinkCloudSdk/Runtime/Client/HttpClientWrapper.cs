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
        Task<TResult> PostAsync<TBody, TResult>(string requestUri, TBody body, CancellationToken cancellationToken);
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

            return await GetContent<T>(response);
        }

        public async Task<TResult> PostAsync<TBody, TResult>(string requestUri, TBody body, CancellationToken cancellationToken)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(body));
            var response = await _innerClient.PostAsync(requestUri, requestContent, cancellationToken);

            return await GetContent<TResult>(response);
        }

        private async Task<T> GetContent<T>(HttpResponseMessage response)
        {
            response.EnsureCloudServiceSuccess();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}