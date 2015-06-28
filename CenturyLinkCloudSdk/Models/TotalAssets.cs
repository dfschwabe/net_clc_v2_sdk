namespace CenturyLinkCloudSdk.Models
{
    public class TotalAssets
    {
        public int Servers { get; set; }

        public int Cpus { get; set; }

        public int MemoryGB { get; set; }

        public int StorageGB { get; set; }

        public int Queue { get; set; }

        public AssetMeasure Memory { get; set; }

        public AssetMeasure Storage { get; set; }
    }

    public class AssetMeasure
    {
        public string Asset { get; set; }

        public int Total { get; set; }

        public string UnitOfMeasure { get; set; }
    }
}