using System;
using System.Collections.Generic;
using System.Linq;
using CenturyLinkCloudSdk.UAT.Mock.Controllers;

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
                RecentActivity = new List<MockActivity>(),
                DataCentersById = new Dictionary<string, MockDataCenter>(),
                AlertPolicies = new Dictionary<string, MockAlertPolicy>()
            };

            UserB = new User
            {
                Username = B,
                Password = "passB",
                AccountAlias = "aliasB",
                RecentActivity = new List<MockActivity>(),
                DataCentersById = new Dictionary<string, MockDataCenter>(),
                AlertPolicies = new Dictionary<string, MockAlertPolicy>()
            };

            All = new List<User> { UserA, UserB };
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

        public static void Reset()
        {
            All.ForEach(u =>
            {
                u.AlertPolicies.Clear();
                u.DataCentersById.Clear();
                u.RecentActivity.Clear();
            });
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountAlias { get; set; }
        public List<MockActivity> RecentActivity { get; set; }
        public Dictionary<string, MockDataCenter> DataCentersById { get; set; }
        public Dictionary<string,MockAlertPolicy> AlertPolicies { get; set; } 
    }

    public class MockActivity
    {
        public string AccountAlias { get; set; }

        public string Body { get; set; }

        public string AccountDescription { get; set; }

        public int EntityId { get; set; }

        public string ReferenceId { get; set; }

        public string EntityType { get; set; }

        public string LocationAlias { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Subject { get; set; }
    }
}