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
                AccountAlias = "aliasA"
            };
        }

        public static User UserA { get; private set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountAlias { get; set; }
    }
}