using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CenturyLinkCloudSdk.Runtime.Client
{
    public class JsonMediaTypeHandler : DelegatingHandler
    {
        private const string Json = "application/json";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Json));

            if (request.Content != null)
            {
                request.Content.Headers.ContentType.MediaType = Json;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}