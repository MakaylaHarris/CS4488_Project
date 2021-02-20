using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SmartPert.Model
{
    /// <summary>
    /// DBReader Singleton checks updates from the database and updates the entire model
    /// Can be used to check database connection with DBReader.Connected
    /// Robert Nelson 1/28/2021
    /// </summary>
    public class DBReader
    {
        #region private fields
        private static readonly DBReader instance = new DBReader();
        private Project currentProject;
        private DBUpdateReceiver receiver;
        private Dictionary<int, Project> projects;
        private User currentUser;
        private Dictionary<string, User> users;
        private Dictionary<int, Task> tasks;
        private DBPoller polling;
        private SqlConnection connection;
        private bool isUpdating;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if connected to database
        /// </summary>
        public bool Connected { get => instance.polling.Running;  }
        static public DBReader Instance { get => instance; }

        /// <summary>
        /// Is db reader updating model?
        /// </summary>
        public bool IsUpdating { get => isUpdating; }


        internal Project CurrentProject { get => currentProject; }
        internal Dictionary<int, Project> Projects { get => projects; }
        internal Dictionary<string, User> Users { get => users; }

        internal Dictionary<int, Task> Tasks { get => tasks; }

        public User CurrentUser {get => currentUser; }
        #endregion

        #region Constructor
        /// <summary>
        /// This initiates the receiver
        /// </summary>
        /// <param name="receiver">The update receiver</param>
        /// <returns>DBReader instance</returns>
        public static DBReader Instantiate(DBUpdateReceiver receiver)
        {
            instance.TestNewConnection(Properties.Settings.Default.ConnectionString);
            instance.receiver = receiver;
            return instance;
        }

        private DBReader()
        {
            polling = new DBPoller(this);
            users = new Dictionary<string, User>();
            projects = new Dictionary<int, Project>();
            tasks = new Dictionary<int, Task>();
            isUpdating = false;
        }
        #endregion

        #region Private Methods
        private SqlDataReader OpenReader(string query)
        {
            SqlCommand command = new SqlCommand(query, connection);
            return command.ExecuteReader();
        }

        private SqlDataReader ReadTable(string table)
        {
            return OpenReader("Select * From " + table + ";");
        }

        private SqlCommand OpenAndGetCmd(string cmd)
        {
            connection.Open();
            return new SqlCommand(cmd, connection);
        }

        private void UpdateProject()
        {
            projects.Clear();
            SqlDataReader reader = ReadTable("Project");
            string name = Properties.Settings.Default.LastProject;
            while (reader.Read())
            {
                Project p = Project.Parse(reader, users);
                projects.Add(p.Id, p);
                if (currentProject != null)
                {
                    if (p.Id == currentProject.Id)
                        currentProject = p;
                }   // Detect the last project
                else if (name != "" && p.Name == name)
                    currentProject = p;
            }
            reader.Close();
        }

        /// <summary>
        /// Gets all the tasks
        /// </summary>
        /// <returns>Dictionary of task id to tasks</returns>
        private Dictionary<int, Task> UpdateTasks()
        {
            tasks.Clear();
            SqlDataReader reader = OpenReader("Select * from Task;");
            while (reader.Read())
            {
                Task t = Task.Parse(reader, users, projects);
                tasks.Add(t.Id, t);
            }
            reader.Close();
            return tasks;
        }

        private void UpdateUsers()
        {
            users.Clear();
            SqlDataReader reader = ReadTable("[User]");
            while (reader.Read())
            {
                User u = User.Parse(reader);
                users.Add(u.Username, u);
            }
            reader.Close();
        }

        /// <summary>
        /// Implemented 2/4/2021 by Robert Nelson
        /// </summary>s
        private void UpdateDependencies(Dictionary<int, Task> idToTask)
        {
            SqlDataReader reader = ReadTable("Dependency");
            while(reader.Read())
            {
                int rootId = (int) reader["RootId"];
                int dependentId = (int)reader["DependentId"];
                idToTask[rootId].AddDependency(idToTask[dependentId]);
            }
            reader.Close();
        }

        private void UpdateWorkers(Dictionary<int, Task> idToTask)
        {            
            // Read and update task workers
            SqlDataReader reader = ReadTable("UserTask");
            Task t;
            while(reader.Read())
            {
                int taskId = (int)reader["TaskId"];
                string username = (string)reader["UserName"];
                if (idToTask.TryGetValue(taskId, out t))
                    t.AddWorker(users[username], false);
            }
            reader.Close();

            // Finally, Read and update project workers (current project only)
            reader = OpenReader("Select * From UserProject Where ProjectId=" + currentProject.Id + ";");
            while(reader.Read())
            {
                currentProject.AddWorker(users[(string)reader["UserName"]], false);
            }
            reader.Close();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines if name is a valid username, must not be empty, contain an @, or be already used
        /// </summary>
        /// <param name="name">the name to test</param>
        /// <returns>true</returns>
        public bool IsValidUsername(string name)
        {
            if(name != "" && !name.Contains("@"))
            {
                SqlCommand command = OpenAndGetCmd("SELECT COUNT(*) FROM [USER] WHERE [USER].UserName=@username AND Password IS NOT Null AND PASSWORD !='';");
                command.Parameters.AddWithValue("@username", name);
                int result = (int)command.ExecuteScalar();
                connection.Close();
                return result == 0;
            }
            return false;
        }

        /// <summary>
        /// Determines if an email is valid
        /// </summary>
        /// <param name="email">email address</param>
        /// <returns>true if valid</returns>
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if(addr.Address == email)
                {
                    SqlCommand command = OpenAndGetCmd("SELECT COUNT(*) FROM [USER] WHERE [USER].Email=@email;");
                    command.Parameters.AddWithValue("@email", email);
                    int result = (int)command.ExecuteScalar();
                    connection.Close();
                    return result == 0;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Attempts to Login
        /// </summary>
        /// <param name="userId">UserName or Email</param>
        /// <param name="password">Password</param>
        /// <returns>True on success</returns>
        public bool Login(string userId, string password)
        {
            SqlCommand command = OpenAndGetCmd("EXECUTE [dbo].TryLogin @userId, @password, @success out;");
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@password", password);
            var success = command.Parameters.Add("@success", System.Data.SqlDbType.Bit);
            success.Direction = System.Data.ParameterDirection.Output;
            command.ExecuteNonQuery();
            bool result = (bool) success.Value;
            connection.Close();
            if (result)
            {
                bool isUserName = false;
                User u;
                if(users.TryGetValue(userId, out u))
                {
                    isUserName = true;
                    currentUser = u;
                }
                if(!isUserName)
                    foreach(User user in users.Values)
                        if(user.Email == userId)
                        {
                            currentUser = user;
                            break;
                        }
                if(userId != Properties.Settings.Default.UserName)
                {
                    Properties.Settings.Default.UserName = userId;
                    Properties.Settings.Default.Save();
                }
            }
            return result;
        }

        /// <summary>
        /// Registers user if possible
        /// </summary>
        /// <param name="username">unique username</param>
        /// <param name="password">password</param>
        /// <param name="name">actual name</param>
        /// <param name="email">email address</param>
        /// <returns>true on success</returns>
        public bool Register(string username, string password, string name, string email)
        {
            User user = new User(name, email, password, username);
            if(user.Register())
            {
                currentUser = user;
                Properties.Settings.Default.UserName = currentUser.Username;
                Properties.Settings.Default.Save();
                users.Add(user.Username, user);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Used to create a user with just a name (unregistered)
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>null if it failed, user on success</returns>
        public User CreateUser(string name)
        {
            User user = new User(name);
            if (!user.Register())
                return null;
            users.Add(user.Username, user);
            return user;
        }

        /// <summary>
        /// Deletes the user. User must be logged in to delete a registered user.
        /// </summary>
        /// <param name="username">user to delete</param>
        /// <returns>true on success</returns>
        public bool DeleteUser(string username)
        {
            User user;
            if(users.TryGetValue(username, out user))
            {
                if(user == currentUser || user.Password == "" || user.Password == null)
                {
                    user.Delete();
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Tests a new connection
        /// </summary>
        /// <param name="connectString">string to connect to database</param>
        /// <returns>true if connection succeeds</returns>
        public bool TestNewConnection(string connectString)
        {
            try
            {
                connection = new SqlConnection(connectString);
                connection.Open();
                connection.Close();
            } catch(Exception e)
            {
                Console.WriteLine(e);
                polling.Stop();
                return false;
            }
            if (connectString != Properties.Settings.Default.ConnectionString)
            {
                Properties.Settings.Default.ConnectionString = connectString;
                Properties.Settings.Default.Save();
            }
            if (Connected)
            {
                polling.Reset();
            }
            else
                polling.Start();
            OnDBUpdate();
            return true;
        }

        /// <summary>
        /// Gets all the projects
        /// </summary>
        /// <returns>list of projects</returns>
        public Dictionary<int, Project> GetUserProjects()
        {
            // Currently, this only returns all projects and not those the user is member to
            // future todo is to let users be invited to project and only get those the user is a part of
            return projects;
        }


        /// <summary>
        /// This updates the entire model and sends the update to the receiver.
        /// </summary>
        public void OnDBUpdate()
        {
            isUpdating = true;
            connection.Open();
            UpdateUsers();
            UpdateProject();
            if (CurrentProject != null)
            {
                Dictionary<int, Task> idToTask = UpdateTasks();
                UpdateDependencies(idToTask);
                UpdateWorkers(idToTask);
            }
            connection.Close();
            isUpdating = false;
            if(receiver != null)
                receiver.OnDBUpdate(CurrentProject);
        }

        public void OnDBDisconnect()
        {
            currentUser = null;
        }

        /// <summary>
        /// Sets the current project which triggers a model update
        /// </summary>
        /// <param name="project">the project</param>
        public void SetProject(Project project)
        {
            currentProject = project;
            if(project.Name != Properties.Settings.Default.LastProject)
            {
                Properties.Settings.Default.LastProject = project.Name;
                Properties.Settings.Default.Save();
            }
            OnDBUpdate();
        }
        #endregion
    }
}
