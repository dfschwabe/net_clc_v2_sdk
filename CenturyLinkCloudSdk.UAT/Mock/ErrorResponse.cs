using System.Collections.Generic;
using System.Net;

namespace CenturyLinkCloudSdk.UAT.Mock
{
    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public ErrorReason Reason { get; set; }
    }
    public class ErrorReason
    {
        public string message { get; set; }
        public Dictionary<string,string[]> modelState { get; set; }
    }
}