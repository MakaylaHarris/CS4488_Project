﻿using System.Data.SqlClient;

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


        #region Constructor
        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <param name="username">Username (uses name if empty)</param>
        /// <param name="register">Registers the user</param>
        /// <param name="track">Track user (must be enabled for updates)</param>
        /// <param name="observer">Observer</param>
        public User(string name, string email = "", string password = "", string username = "", bool register=true, bool track=true, IItemObserver observer=null) : base(observer)
        {
            this.name = name;
            this.email = email;
            this.password = password;
            if (username == "")
                this.username = name;
            else
                this.username = username;
            PostInit(register, track);
        }
        #endregion

        public override string ToString() => username;

        #region Database Methods
        /// <summary>
        /// Deletes user from database
        /// </summary>
        protected override void PerformDelete()
        {
            using (var conn = new DBConnection("Delete from [User] Where [User].[UserName]=@username;"))
            {
                SqlCommand command = conn.Command;
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Attempts to register user
        /// </summary>
        /// <throws>InsertionError if registration failed</throws>
        protected override int PerformInsert()
        {
            bool registered = false;
            try
            {
                using (var conn = new DBConnection("Exec dbo.Register @username, @password, @email, @name, @result out"))
                {
                    SqlCommand command = conn.Command;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@name", name);
                    var result = command.Parameters.Add("@result", System.Data.SqlDbType.Bit);
                    result.Direction = System.Data.ParameterDirection.Output;
                    command.ExecuteNonQuery();
                    registered = (bool)result.Value;
                }
            }
            catch (SqlException) { }
            if (!registered)
                throw new InsertionError("Failed to register " + username);
            return 0;
        }

        /// <summary>
        /// Updates the user's name email and password
        /// </summary>
        protected override void PerformUpdate()
        {
            using (var conn = new DBConnection("Update [User] set [Name]=@name, Email=@email, [Password]=@password WHERE UserName=@username"))
            {
                SqlCommand command = conn.Command;
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
            }
        }

        static public User Parse(SqlDataReader reader)
        {
            return new User((string)reader["Name"],
                (string)reader["Email"],
                (string)reader["Password"],
                (string)reader["UserName"], register: false);
        }

        /// <summary>
        /// Parses in the data from database
        /// </summary>
        /// <param name="reader">reader</param>
        /// <returns>true if updated</returns>
        public override bool PerformParse(SqlDataReader reader)
        {
            string name = (string)reader["Name"];
            string email = (string)reader["Email"];
            string pass = (string)reader["Password"];
            if(name != this.name || email != this.email || pass != this.password)
            {
                this.name = name;
                this.email = email;
                this.password = pass;
                return true;
            }
            return false;
        }

        #endregion
    }
}
