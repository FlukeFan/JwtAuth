namespace AuthEx.Security.Areas.Identity.Data
{
    public static class UserData
    {
        public static class User
        {
            public const string Username = "user@test.com";
            public const string Password = "user_Pa55w0rd";
        }

        public static class Admin
        {
            public const string Username = "admin@test.com";
            public const string Password = "admin_Pa55w0rd";
        }

        public static class Roles
        {
            public const string Users = "users";
            public const string Admins = "admins";
        }
    }
}
