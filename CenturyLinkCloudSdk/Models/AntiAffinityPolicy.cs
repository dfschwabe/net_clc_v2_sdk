namespace CenturyLinkCloudSdk.Models
{
    public class AntiAffinityPolicyDefinition
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }

    public class AntiAffinityPolicy : AntiAffinityPolicyDefinition
    {
        public string Id { get; set; }
    }
}