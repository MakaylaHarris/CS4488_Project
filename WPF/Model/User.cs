using System.Data.SqlClient;

namespace SmartPert.Model
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

        public override string ToString() => username;

        /// <summary>
        /// Attempts to register user
        /// </summary>
        /// <returns>true on success</returns>
        public bool Register()
        {
            SqlCommand command = OpenConnection("Exec dbo.Register @username, @password, @email, @name, @result out");
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@name", name);
            var result = command.Parameters.Add("@result", System.Data.SqlDbType.Bit);
            result.Direction = System.Data.ParameterDirection.Output;
            command.ExecuteNonQuery();
            CloseConnection();
            return (bool) result.Value;
        }

        
        #region Database Methods
        /// <summary>
        /// Deletes user from database
        /// </summary>
        public override void Delete()
        {
            SqlCommand command = OpenConnection("Delete from [User] Where [User].[UserName]=@username;");
            command.Parameters.AddWithValue("@username", username);
            command.ExecuteNonQuery();
            CloseConnection();
        }

        /// <summary>
        /// Not used, use register instead.
        /// </summary>
        protected override int Insert()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates the user's name email and password
        /// </summary>
        protected override void Update()
        {
            SqlCommand command = OpenConnection("Update [User] set [Name]=@name, Email=@email, [Password]=@password WHERE UserName=@username");
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@username", username);
            command.ExecuteNonQuery();
            CloseConnection();
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
