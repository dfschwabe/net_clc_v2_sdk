﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CenturyLinkCloudSdk.Extensions;
using Newtonsoft.Json;

namespace CenturyLinkCloudSdk.Runtime.Client
{
    public interface IHttpClient
    {
        Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken);
        Task<TResult> PostAsync<TResult>(string requestUri, object body, CancellationToken cancellationToken);
        Task DeleteAsync(string requestUri, CancellationToken expectedToken);
        Task<T> PutAsync<T>(string requestUri, object body, CancellationToken cancellationToken);
    }

    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _innerClient;
        private readonly JsonSerializerSettings _serializerSettings;

        public HttpClientWrapper(HttpClient innerClient, JsonSerializerSettings serializerSettings)
        {
            _innerClient = innerClient;
            _serializerSettings = serializerSettings;
        }

        public async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
        {
            var response = await _innerClient.GetAsync(requestUri, cancellationToken);

            return await GetContent<T>(response);
        }

        public async Task<T> PostAsync<T>(string requestUri, object body, CancellationToken cancellationToken)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(body, _serializerSettings));
            var response = await _innerClient.PostAsync(requestUri, requestContent, cancellationToken);

            return await GetContent<T>(response);
        }

        public async Task DeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            var response = await _innerClient.DeleteAsync(requestUri, cancellationToken);

            response.EnsureCloudServiceSuccess();
        }

        public async Task<T> PutAsync<T>(string requestUri, object body, CancellationToken cancellationToken)
        {
            var requestContent = new StringContent(JsonConvert.SerializeObject(body, _serializerSettings));
            var response = await _innerClient.PutAsync(requestUri, requestContent, cancellationToken);

            return await GetContent<T>(response);
        }

        private async Task<T> GetContent<T>(HttpResponseMessage response)
        {
            response.EnsureCloudServiceSuccess();

            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}