using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WPF.Model
{
    /// <summary>
    /// DBReader Singleton checks updates from the database and updates the entire model
    /// Can be used to check database connection with DBReader.Connected
    /// Robert Nelson 1/28/2021
    /// </summary>
    class DBReader
    {
        #region private fields
        private static readonly DBReader instance = new DBReader();
        private Project currentProject;
        private DBUpdateReceiver receiver;
        private List<Project> projects;
        private List<User> users;
        private DBPoller polling;
        private SqlConnection connection;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if connected to database
        /// </summary>
        public bool Connected { get => instance.polling.Running;  }
        static public DBReader Instance { get => instance; }


        internal Project CurrentProject { get => currentProject; }
        internal List<Project> Projects { get => projects; }
        internal List<User> Users { get => users; }
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


        private void UpdateProject()
        {
            projects = new List<Project>();
            SqlDataReader reader = ReadTable("Project");
            string name = Properties.Settings.Default.LastProject;
            while (reader.Read())
            {
                Project p = Project.Parse(reader);
                projects.Add(p);
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
        /// Gets all the tasks on the current project
        /// </summary>
        /// <returns>Dictionary of task id to tasks</returns>
        private Dictionary<int, Task> UpdateTasks()
        {
            Dictionary<int, Task> idToTask = new Dictionary<int, Task>();
            SqlDataReader reader = OpenReader("Select * from Task Where ProjectId=" + CurrentProject.Id + ";");
            while (reader.Read())
            {
                Task t = Task.Parse(reader);
                idToTask[t.Id] = t;
                CurrentProject.AddTask(t);
            }
            reader.Close();
            return idToTask;
        }

        private void UpdateUsers()
        {
            users = new List<User>();
            SqlDataReader reader = ReadTable("[User]");
            while (reader.Read())
                users.Add(User.Parse(reader));
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
            // First, Create dictionaries to access everything by id
            Dictionary<int, Project> idToProject = new Dictionary<int, Project>();
            Dictionary<string, User> idToUser = new Dictionary<string, User>();
            foreach (Project p in projects)
                idToProject[p.Id] = p;
            foreach (User user in users)
                idToUser[user.Username] = user;
            
            // Next, Read and update task workers
            SqlDataReader reader = ReadTable("UserTask");
            Task t;
            while(reader.Read())
            {
                int taskId = (int)reader["TaskId"];
                string username = (string)reader["UserName"];
                if (idToTask.TryGetValue(taskId, out t))
                    t.AddWorker(idToUser[username]);
            }
            reader.Close();

            // Finally, Read and update project workers (current project only)
            reader = OpenReader("Select * From UserProject Where ProjectId=" + currentProject.Id + ":");
            while(reader.Read())
            {
                currentProject.AddWorker(idToUser[(string)reader["UserName"]]);
            }
            reader.Close();
        }
        #endregion

        #region Public Methods
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
                polling.Reset();
            else
                polling.Start();
            return true;
        }

        /// <summary>
        /// Gets all the projects
        /// </summary>
        /// <returns>list of projects</returns>
        public List<Project> GetUserProjects()
        {
            // Currently, this only returns all projects and not those the user is member to
            // future todo is to let users be invited to project and only get those the user is a part of
            return Projects;
        }


        /// <summary>
        /// This updates the entire model and sends the update to the receiver.
        /// </summary>
        public void OnDBUpdate()
        {
            connection.Open();
            UpdateProject();
            UpdateUsers();
            if (CurrentProject != null)
            {
                Dictionary<int, Task> idToTask = UpdateTasks();
                UpdateDependencies(idToTask);
                UpdateWorkers(idToTask);
            }
            connection.Close();
            receiver.OnDBUpdate(CurrentProject);
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
