using System;
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
            Request = String.Format("{0}:{1}", response.RequestMessage.Method, response.RequestMessage.RequestUri);
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }
        public string Request { get; private set; }
    }
}