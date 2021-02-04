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
        private static readonly DBReader instance = new DBReader();
        private Project currentProject;
        private DBUpdateReceiver receiver;
        private List<Project> projects;
        private List<User> users;
        private DBPoller polling;
        private SqlConnection connection;

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
            bool connected = instance.TestNewConnection(Properties.Settings.Default.ConnectionString);
            string lastProject = Properties.Settings.Default.LastProject;
            if (lastProject != "" && connected)
            {
                instance.UpdateProject();
                foreach (Project p in instance.projects)
                {
                    if (p.Name == lastProject)
                        instance.currentProject = p;
                }
            }
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
            connection.Open();
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
            while (reader.Read())
            {
                Project p = Project.Parse(reader);
                projects.Add(p);
                if (CurrentProject != null && p.Id == CurrentProject.Id)
                    currentProject = p;
            }
            connection.Close();
        }

        private void UpdateTasks()
        {
            SqlDataReader reader = OpenReader("Select * from Task Where ProjectId=" + CurrentProject.Id + ";");
            while (reader.Read())
            {
                CurrentProject.AddTask(Task.Parse(reader));
            }
            connection.Close();
        }

        private void UpdateUsers()
        {
            users = new List<User>();
            SqlDataReader reader = ReadTable("[User]");
            while (reader.Read())
            {
                users.Add(User.Parse(reader));
            }
            connection.Close();
        }

        private void UpdateDependencies()
        {
            throw new NotImplementedException();
        }

        private void UpdateWorkers()
        {
            // todo: Update those working on each task and project
            throw new NotImplementedException();
        }
        #endregion

        #region Public Methods
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
            if(connectString != Properties.Settings.Default.ConnectionString)
                Properties.Settings.Default.ConnectionString = connectString;
            if (Connected)
                polling.Reset();
            else
                polling.Start();
            return true;
        }

        public List<Project> GetUserProjects()
        {
            // Currently, this only returns all projects and not those the user is member to
            return Projects;
        }

        public void OnDBUpdate()
        {
            UpdateProject();
            UpdateUsers();
            if (CurrentProject != null)
            {
                UpdateTasks();
                UpdateDependencies();
                UpdateWorkers();
            }
            receiver.OnDBUpdate(CurrentProject);
        }

        public void SetProject(Project project)
        {
            currentProject = project;
            Properties.Settings.Default.LastProject = project.Name;
            OnDBUpdate();
        }
        #endregion
    }
}
