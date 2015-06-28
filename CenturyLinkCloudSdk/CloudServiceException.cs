using System.Net;
using System.Net.Http;

namespace CenturyLinkCloudSdk
{
    public class CloudServiceException : System.Exception
    {
        public CloudServiceException(HttpResponseMessage response)
        {
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }
    }
}