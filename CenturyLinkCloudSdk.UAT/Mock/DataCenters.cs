namespace CenturyLinkCloudSdk.UAT.Mock
{
    public static class DataCenters
    {
        static DataCenters()
        {
            DCA = new MockDataCenter
            {
                Id = "dca",
                Name = "dca_name",
                Totals = new MockTotalAssets
                {
                    Servers = 1000,
                    Cpus = 4000,
                    MemoryGB = 128000,
                    StorageGB = 8000000,
                    Queue = 9
                }
            };

            DCB = new MockDataCenter
            {
                Id = "dcb",
                Name = "dcb_name",
                Totals = new MockTotalAssets
                {
                    Servers = 2000,
                    Cpus = 8000,
                    MemoryGB = 256000,
                    StorageGB = 16000000,
                    Queue = 18
                }
            };
        }

        public static MockDataCenter DCA { get; private set; }
        public static MockDataCenter DCB { get; private set; }
    }

    public class MockDataCenter
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public MockTotalAssets Totals { get; set; }
    }

    public class MockTotalAssets
    {
        public int Servers { get; set; }

        public int Cpus { get; set; }

        public int MemoryGB { get; set; }

        public int StorageGB { get; set; }

        public int Queue { get; set; }
    }
}