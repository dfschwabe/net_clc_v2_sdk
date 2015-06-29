using System.Collections.Generic;

namespace CenturyLinkCloudSdk.UAT.Mock
{
    public static class Users
    {
        static Users()
        {
            UserA = new User
            {
                Username = "userA",
                Password = "passA",
                AccountAlias = "aliasA",
                DataCentersById = new Dictionary<string, MockDataCenter>
                {
                    {DataCenters.DCA.Id, DataCenters.DCA},
                    {DataCenters.DCB.Id, DataCenters.DCB},
                }
            };

            UsersByAccountAlias = new Dictionary<string, User>
            {
                {UserA.AccountAlias, UserA}
            };
        }

        public static User UserA { get; private set; }
        public static Dictionary<string, User> UsersByAccountAlias { get; private set; }

    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountAlias { get; set; }
        public Dictionary<string, MockDataCenter> DataCentersById { get; set; } 
    }
}