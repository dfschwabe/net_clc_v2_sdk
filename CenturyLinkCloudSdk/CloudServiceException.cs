using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CenturyLinkCloudSdk.Models.Internal;
using Newtonsoft.Json;

namespace CenturyLinkCloudSdk
{
    public class CloudServiceException : Exception
    {
        public CloudServiceException(HttpResponseMessage response)
        {
            Request = String.Format("{0}:{1}", response.RequestMessage.Method, response.RequestMessage.RequestUri);
            StatusCode = response.StatusCode;
            ReasonPhrase = response.ReasonPhrase;

            try
            {
                var reason = JsonConvert.DeserializeObject<ErrorReason>(response.Content.ReadAsStringAsync().Result);
                ErrorMessage = reason.Message;
                ValidationErrors = reason.ModelState;
            }
            catch(Exception) { }
        }

        public HttpStatusCode StatusCode { get; private set; }
        public string ReasonPhrase { get; private set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string,string[]> ValidationErrors { get; set; }
        public string Request { get; private set; }
    }
}