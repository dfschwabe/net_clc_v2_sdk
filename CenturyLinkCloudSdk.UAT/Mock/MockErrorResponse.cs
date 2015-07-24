using System.Collections.Generic;
using System.Net;

namespace CenturyLinkCloudSdk.UAT.Mock
{
    public class MockErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public MockErrorReason Reason { get; set; }
    }
    public class MockErrorReason
    {
        public string message { get; set; }
        public Dictionary<string,string[]> modelState { get; set; }
    }
}