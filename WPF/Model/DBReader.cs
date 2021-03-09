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
    public class DBReader : IItemObserver
    {
        #region private fields
        private static readonly DBReader instance = new DBReader();
        private Project currentProject;
        private DBUpdateReceiver receiver;
        private User currentUser;
        // Current copy of the data
        private Dictionary<int, Project> projects;
        private Dictionary<string, User> users;
        private Dictionary<int, Task> tasks;
        private Dictionary<int, HashSet<User>> projectWorkers;
        private Dictionary<int, HashSet<User>> taskWorkers;
        private Dictionary<int, HashSet<Task>> dependencies;
        private Dictionary<int, HashSet<Task>> subtasks;

        private DBPoller polling;
        private SqlConnection connection;
        private bool isUpdating;
        private int updateCount;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of updates sent to the receiver
        /// </summary>
        public int UpdateCount { get => updateCount; }
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
            instance.receiver = receiver;
            instance.TestNewConnection(Properties.Settings.Default.ConnectionString);
            return instance;
        }

        private DBReader()
        {
            polling = new DBPoller(this);
            users = new Dictionary<string, User>();
            projects = new Dictionary<int, Project>();
            tasks = new Dictionary<int, Task>();
            projectWorkers = new Dictionary<int, HashSet<User>>();
            taskWorkers = new Dictionary<int, HashSet<User>>();
            dependencies = new Dictionary<int, HashSet<Task>>();
            subtasks = new Dictionary<int, HashSet<Task>>();
            isUpdating = false;
        }
        #endregion

        #region Private Methods
        private SqlDataReader OpenReader(string query)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
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
            Project p;
            Dictionary<int, bool> found = new Dictionary<int, bool>();
            foreach(Project proj in projects.Values)
                found[proj.Id] = false;
            SqlDataReader reader = ReadTable("Project");
            string name = Properties.Settings.Default.LastProject;
            while (reader.Read())
            {
                if (projects.TryGetValue((int)reader["ProjectId"], out p))
                {
                    p.ParseUpdate(reader);
                    found[p.Id] = true;
                }
                else
                {
                    p = Project.Parse(reader, users);
                }
                if (currentProject != null)
                {
                    if (p.Id == currentProject.Id)
                        currentProject = p;
                }   // Detect the last project
                else if (name != "" && p.Name == name)
                    currentProject = p;
            }
            reader.Close();
            foreach (KeyValuePair<int, bool> keyVal in found)
                if (!keyVal.Value)
                    projects[keyVal.Key].IsDeleted = true;
        }

        /// <summary>
        /// Gets all the tasks
        /// </summary>
        /// <returns>Dictionary of task id to tasks</returns>
        private Dictionary<int, Task> UpdateTasks()
        {
            Task t;
            Dictionary<int, bool> found = new Dictionary<int, bool>();
            foreach (Task task in tasks.Values)
                found[task.Id] = false;
            SqlDataReader reader = OpenReader("Select * from Task ORDER BY ProjectRow DESC;");
            while (reader.Read())
            {
                if(tasks.TryGetValue((int) reader["TaskId"], out t))
                {
                    t.ParseUpdate(reader);
                    found[t.Id] = true;
                } else
                {
                    t = Task.Parse(reader, users, projects);
                }
            }
            reader.Close();
            foreach(KeyValuePair<int, bool> keyValue in found)
                if(!keyValue.Value)
                    tasks[keyValue.Key].IsDeleted = true;
            return tasks;
        }

        private void UpdateUsers()
        {
            User u;
            Dictionary<string, bool> found = new Dictionary<string, bool>();
            foreach (string s in users.Keys)
                found[s] = false;
            SqlDataReader reader = ReadTable("[User]");
            while (reader.Read())
            {
                string username = (string)reader["UserName"];
                if (users.TryGetValue(username, out u))
                {
                    u.ParseUpdate(reader);
                    found[username] = true;
                }
                else
                {
                    u = User.Parse(reader);
                }
            }
            reader.Close();
            foreach (KeyValuePair<string, bool> keyValue in found)
                if (!keyValue.Value)
                    users[keyValue.Key].IsDeleted = true;
        }

        /// <summary>
        /// Implemented 2/4/2021 by Robert Nelson
        /// </summary>
        private void UpdateDependencies(Dictionary<int, Task> idToTask)
        {
            // Grab the new data
            Dictionary<int, HashSet<Task>> newDepend = new Dictionary<int, HashSet<Task>>();
            HashSet<Task> tmp;
            SqlDataReader reader = ReadTable("Dependency");
            while(reader.Read())
            {
                int rootId = (int) reader["RootId"];
                if (!newDepend.TryGetValue(rootId, out tmp))
                    newDepend[rootId] = tmp = new HashSet<Task>();
                tmp.Add(idToTask[(int)reader["DependentId"]]);
            }
            reader.Close();

            // Now update any removed dependencies
            foreach(int key in dependencies.Keys)
            {
                if (!newDepend.ContainsKey(key))
                    if(idToTask.ContainsKey(key))
                        idToTask[key].DB_UpdateDependencies(null);
            }
            Task task;
            // And update new task dependencies
            foreach(KeyValuePair<int, HashSet<Task>> keyValue in newDepend)
            {
                task = idToTask[keyValue.Key];
                task.DB_UpdateDependencies(keyValue.Value);
            }
            dependencies = newDepend;
        }

        private void UpdateSubtasks(Dictionary<int, Task> idToTask)
        {
            // Grab the new data
            Dictionary<int, HashSet<Task>> newSubtasks = new Dictionary<int, HashSet<Task>>();
            HashSet<Task> tmp;
            SqlDataReader reader = ReadTable("SubTask");
            while (reader.Read())
            {
                int rootId = (int)reader["ParentTaskId"];
                if (!newSubtasks.TryGetValue(rootId, out tmp))
                    newSubtasks[rootId] = tmp = new HashSet<Task>();
                tmp.Add(idToTask[(int)reader["SubTaskId"]]);
            }
            reader.Close();

            // Now update any removed subtasks
            foreach (int key in subtasks.Keys)
            {
                if (!newSubtasks.ContainsKey(key))
                    if (idToTask.ContainsKey(key))
                        idToTask[key].DB_UpdateSubtasks(null);
            }
            Task task;
            // And update new/changed subtasks
            foreach (KeyValuePair<int, HashSet<Task>> keyValue in newSubtasks)
            {
                task = idToTask[keyValue.Key];
                task.DB_UpdateSubtasks(keyValue.Value);
            }
            subtasks = newSubtasks;

        }

        private Dictionary<int, HashSet<User>> ReadWorkerData(string table, string id)
        {
            // Read in task workers
            Dictionary<int, HashSet<User>> newWorkers = new Dictionary<int, HashSet<User>>();
            HashSet<User> tmp;
            SqlDataReader reader = ReadTable(table);
            while (reader.Read())
            {
                int id_val = (int)reader[id];
                if (!newWorkers.TryGetValue(id_val, out tmp))
                    newWorkers[id_val] = tmp = new HashSet<User>();
                tmp.Add(users[(string)reader["UserName"]]);
            }
            reader.Close();
            return newWorkers;
        }

        private void UpdateTaskWorkers()
        {
            Dictionary<int, HashSet<User>> newWorkers = ReadWorkerData("UserTask", "TaskId");
            foreach (int key in taskWorkers.Keys)
                if (!newWorkers.ContainsKey(key))
                    tasks[key].DB_UpdateWorkers(null);
            foreach (KeyValuePair<int, HashSet<User>> keyValue in newWorkers)
                tasks[keyValue.Key].DB_UpdateWorkers(keyValue.Value);
        }

        private void UpdateProjectWorkers()
        {
            Dictionary<int, HashSet<User>> newWorkers = ReadWorkerData("UserProject", "ProjectId");
            foreach (int key in projectWorkers.Keys)
                if (!newWorkers.ContainsKey(key))
                    projects[key].DB_UpdateWorkers(null);
            foreach (KeyValuePair<int, HashSet<User>> keyValue in newWorkers)
                projects[keyValue.Key].DB_UpdateWorkers(keyValue.Value);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a new item for tracking
        /// </summary>
        /// <param name="item">newly created item</param>
        public void TrackItem(IDBItem item)
        {
            if (item.GetType() == typeof(Task))
            {
                Task task;
                Task t = (Task)item;
                if(tasks.TryGetValue(t.Id, out task))
                    if(!ReferenceEquals(task, t))
                        task.IsOutdated = true;
                tasks[t.Id] = t;
            }
            else if (item.GetType() == typeof(Project))
            {
                Project project;
                Project p = (Project)item;
                if (projects.TryGetValue(p.Id, out project))
                    if (!ReferenceEquals(project, p))
                        project.IsOutdated = true;
                projects[p.Id] = p;
            }  else if(item.GetType() == typeof(User))
            {
                User user;
                User u = (User)item;
                if (users.TryGetValue(u.Username, out user))
                    if (!ReferenceEquals(u, user))
                        user.IsOutdated = true;
                users[u.Username] = u;
            }
            item.Subscribe(this);
            if(receiver != null)
                receiver.OnDBUpdate(CurrentProject);
        }

        /// <summary>
        /// Update event of IDBItem
        /// </summary>
        /// <param name="item">an updated item</param>
        public void OnUpdate(IDBItem item)
        {
            if (!isUpdating)
                receiver.OnDBUpdate(currentProject); // Send the update up
        }

        /// <summary>
        /// Delete event that removes item
        /// </summary>
        /// <param name="item">IDBItem</param>
        public void OnDeleted(IDBItem item)
        {
            if (item.GetType() == typeof(Task))
            {
                Task t = (Task)item;
                tasks.Remove(t.Id);
            }
            else if (item.GetType() == typeof(Project))
            {
                Project p = (Project)item;
                if (projects.Remove(p.Id) && p == currentProject)
                    currentProject = null;
            }
            else if (item.GetType() == typeof(User))
            {
                User u = (User)item;
                users.Remove(u.Username);
                if (u == currentUser)
                    currentUser = null;
            }
            if(!isUpdating)
                receiver.OnDBUpdate(CurrentProject);
        }

        /// <summary>
        /// Removes the project
        /// </summary>
        /// <param name="p">project</param>
        public void RemoveProject(Project p)
        {
            projects.Remove(p.Id);
            if (p == currentProject)
                currentProject = null;
        }

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
            try
            {
                User user = new User(name, email, password, username);
                currentUser = user;
                Properties.Settings.Default.UserName = currentUser.Username;
                Properties.Settings.Default.Save();
                if (!users.ContainsKey(user.Username))
                    users.Add(user.Username, user);
                return true;
            }
            catch(IDBItem.DuplicateKeyError) { }
            catch(IDBItem.InsertionError) { }
            return false;
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
                    users.Remove(user.Username);
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
            bool isConnected = true;
            try
            {
                connection = new SqlConnection(connectString);
                connection.Open();
                connection.Close();
            } catch(Exception e)
            {
                Console.WriteLine(e);
                polling.Stop();
                isConnected = false;
            } finally
            {
                if (connectString != Properties.Settings.Default.ConnectionString)
                {
                    Properties.Settings.Default.ConnectionString = connectString;
                    Properties.Settings.Default.Save();
                }
            }
            if(isConnected)
            {
                if (Connected)  // poller connected?
                    polling.Reset();
                else
                    polling.Start();
                OnDBUpdate();
            }
            return isConnected;
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
            Dictionary<int, Task> idToTask = UpdateTasks();
            UpdateDependencies(idToTask);
            UpdateSubtasks(idToTask);
            UpdateProjectWorkers();
            UpdateTaskWorkers();
            connection.Close();

            PostUpdate();
            updateCount++;
            isUpdating = false;
            if(receiver != null)
                receiver.OnDBUpdate(CurrentProject);
        }

        /// <summary>
        /// After an update is finished, send out notifications
        /// </summary>
        public void PostUpdate()
        {
            foreach (Project item in projects.Values)
                item.PostUpdate();
            foreach (Task t in tasks.Values)
                t.PostUpdate();
            foreach (User u in users.Values)
                u.PostUpdate();
        }

        /// <summary>
        /// Disconnect event
        /// </summary>
        public void OnDBDisconnect()
        {
            currentUser = null;
            receiver.OnDBDisconnect();
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
