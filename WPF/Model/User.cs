using System.Data.SqlClient;

namespace WPF.Model
{
    /// <summary>
    /// A single user that works on the project
    /// Created 1/29/2021 by Robert Nelson
    /// </summary>
    public class User : IDBItem
    {
        private string username;
        private string name;
        private string email;
        private string password;

        #region Properties
        public string Name
        {
            get => name;
            set
            {
                name = value;
                Update();
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                Update();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                Update();
            }
        }

        public string Username { get => username; }
        #endregion


        public User(string name, string email = "", string password = "", string username = "")
        {
            this.name = name;
            this.email = email;
            this.password = password;
            if (username == "")
                this.username = name;
            else
                this.username = username;
        }

        #region Database Methods
        protected override void Delete()
        {
            ExecuteSql("Delete from [User] Where UserName= '" + username + "';");
        }

        protected override int Insert()
        {
            return ExecuteSql("INSERT INTO [User](UserName, Name, Email, Password) values('"
                + username + "', '" + name + "', '" + email + "', '" + password + "');");
        }

        protected override void Update()
        {
            ExecuteSql("update [User] set [Name]='" + name
                + "', Email= '" + email
                + "', [Password]= '" + password
                + "' Where UserName = '" + username + "';");
        }

        static public User Parse(SqlDataReader reader)
        {
            return new User((string)reader["Name"],
                (string)reader["Email"],
                (string)reader["Password"],
                (string)reader["UserName"]);
        }
        #endregion
    }
}
