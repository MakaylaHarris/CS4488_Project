using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PERT.Model
{
    /// <summary>
    /// DBReader checks updates from the database and updates the entire model
    /// Robert Nelson 1/28/2021
    /// </summary>
    class DBReader
    {
        private Project currentProject;
        private DBUpdateReceiver receiver;
        private List<Project> projects;
        private List<User> users;
        private DBPoller polling;
        private SqlConnection connection;

        internal Project CurrentProject { get => currentProject; }
        internal List<Project> Projects { get => projects; }
        internal List<User> Users { get => users; }

        public DBReader(DBUpdateReceiver receiver)
        {
            connection = new SqlConnection(Properties.Settings.Default.ConnectionString);
            this.receiver = receiver;
            polling = new DBPoller(this);
        }

        #region Private Methods
        private SqlDataReader OpenReader(string query)
        {
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            return command.ExecuteReader();
        }

        private SqlDataReader ReadTable(string table)
        {
            return OpenReader("Select * from " + table + ";");
        }


        private void UpdateProject()
        {
            projects = new List<Project>();
            SqlDataReader reader = ReadTable("Project");
            while(reader.Read())
            {
                Project p = Project.Parse(reader);
                Projects.Add(p);
                if (p.Id == CurrentProject.Id)
                    currentProject= p;
            }
            connection.Close();
        }

        private void UpdateTasks()
        {
            SqlDataReader reader = OpenReader("Select * from Task Where ProjectId=" + CurrentProject.Id + ";");
            while(reader.Read())
            {
                CurrentProject.AddTask(Task.Parse(reader));
            }
            connection.Close();
        }

        private void UpdateUsers()
        {
            users = new List<User>();
            SqlDataReader reader = ReadTable("User");
            while (reader.Read())
            {
                Users.Add(User.Parse(reader));
            }
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
        public List<Project> GetUserProjects()
        {
            // Currently, this only returns all projects and not those the user is member to
            return Projects;
        }

        public void OnDBUpdate()
        {
            UpdateProject();
            UpdateUsers();
            UpdateTasks();
            UpdateDependencies();
            UpdateWorkers();
            this.receiver.OnDBUpdate(CurrentProject);
        }

        public void SetProject(Project project)
        {
            this.currentProject = project;
            this.OnDBUpdate();
        }
        #endregion
    }
}
