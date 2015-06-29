namespace CenturyLinkCloudSdk.UAT.Mock
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {
            message = "We didn't recognize the username or password you entered. Please try again.";
        }

        public string message { get; set; } 
    }
}