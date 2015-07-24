using System.Collections.Generic;
using Newtonsoft.Json;

namespace CenturyLinkCloudSdk.Models.Internal
{
    public class ErrorReason
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("modelState")]
        public Dictionary<string, string[]> ModelState { get; set; }
    }
}