using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.Model
{
    /// <summary>
    /// A single user that works on the project
    /// Created 1/29/2021 by Robert Nelson
    /// </summary>
    public class User : IDBItem
    {
        private uint id;
        private string name;
        private string email;
        private string password;

        #region Properties
        public string Name { get => name;
            set
            {
                name = value;
                Update();
            }
        }

        public string Email { get => email;
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
        #endregion


        public User(string name, string email="", string password = "", int id = -1)
        {
            this.Name = name;
            this.Email = email;
            this.Password = password;
            if (id < 0)
                this.id = (uint) Insert();
            else
                this.id = (uint) id;
        }

        #region Database Methods
        protected override void Delete()
        {
            ExecuteSql("Delete from User Where Id= " + id + ";");
        }

        protected override int Insert()
        {
            return ExecuteSql("INSERT INTO User(Name, Email, Password) values('"
                + name + "', '" + email + "', '" + password + "');");
        }

        protected override void Update()
        {
            ExecuteSql("update User set Name= '" + name
                + "', Email= '" + email
                + "', Password= '" + password
                + "' Where Id=" + id + ";");
        }

        static public User Parse(SqlDataReader reader)
        {
            return new User((string) reader["Name"], 
                (string) reader["Email"], 
                (string) reader["Password"], 
                (int) reader["UserId"]);
        }
        #endregion
    }
}
