using System.Collections.Generic;
using System.Linq;

namespace CenturyLinkCloudSdk.UAT.Mock
{
    public static class Users
    {
        public const string A = "userA";
        public const string B = "userB";

        static Users()
        {
            UserA = new User
            {
                Username = A,
                Password = "passA",
                AccountAlias = "aliasA",
                DataCentersById = new Dictionary<string, MockDataCenter>
                {
                    {DataCenters.DCA.Id, DataCenters.DCA},
                    {DataCenters.DCB.Id, DataCenters.DCB},
                }
            };

            UserB = new User
            {
                Username = B,
                Password = "passB",
                AccountAlias = "aliasB",
                DataCentersById = new Dictionary<string, MockDataCenter>
                {
                    {DataCenters.DCB.Id, DataCenters.DCB},
                }
            };
            
            All = new List<User>{ UserA, UserB };
            ByUsername = All.ToDictionary(user => user.Username);
            ByAccountAlias = All.ToDictionary(user => user.AccountAlias);
        }

        public static User UserA { get; private set; }
        public static User UserB { get; private set; }
        public static List<User> All { get; private set; }
        public static Dictionary<string, User> ByUsername { get; private set; }
        public static Dictionary<string, User> ByAccountAlias { get; private set; }

        public static User Login(string username, string password)
        {
            return All.FirstOrDefault(x => x.Username.Equals(username) && x.Password.Equals(password));
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountAlias { get; set; }
        public Dictionary<string, MockDataCenter> DataCentersById { get; set; } 
    }
}