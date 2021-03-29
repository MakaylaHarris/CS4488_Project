using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SmartPert.View;

namespace SmartPert.Model
{
    /// <summary>
    /// The model is a singleton pipeline between the view and the database
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    public class Model : IModel, DBUpdateReceiver
    {
        private static readonly Model instance = new Model();
        // This initializes DBReader, which must be done after the instance is defined since updates are immediately sent to model
        private static readonly bool initialized = StartInstance();  
        private List<IViewModel> viewModels;
        private DBReader reader;
        private bool isUpdating;

        #region Model Instance
        /// <summary>
        /// Gets the instance and registers the view model
        /// </summary>
        /// <param name="viewModel">view model to register</param>
        /// <returns>Model instance</returns>
        static public Model GetInstance(IViewModel viewModel)
        {
            instance.Subscribe(viewModel);
            return instance;
        }

        /// <summary>
        /// Gets the model instance
        /// </summary>
        /// <returns>model</returns>
        static public Model Instance { get => instance; }

        private Model()
        {
            viewModels = new List<IViewModel>();
        }
        static private bool StartInstance()
        {
            instance.reader = DBReader.Instantiate(instance);
            return true;
        }
        #endregion

        #region Project Methods
        /// <summary>
        /// Creates the project, if possible
        /// </summary>
        /// <param name="name">project name</param>
        /// <param name="start">start date</param>
        /// <param name="end">end date (optionally null)</param>
        /// <param name="description">project description</param>
        /// <returns>the project or null if creation failed</returns>
        public Project CreateProject(string name, DateTime start, DateTime? end, string description = "")
        {
            // Try to create
            //try
            //{
                Project project = new Project(name, start, end, description);
                reader.SetProject(project);
                return project;
            //}
            //catch (Exception) {}
            return null;
        }

        /// <summary>
        /// Deletes the project from the model
        /// </summary>
        /// <param name="p">project to delete</param>
        public void DeleteProject(Project p)
        {
            reader.RemoveProject(p);
            p.Delete();
        }
        public Project GetProject()
        {
            return reader.CurrentProject;
        }
        public List<Project> GetProjectList()
        {
            return reader.Projects.Values.ToList();
        }

        public void SetProject(Project project)
        {
            reader.SetProject(project);
        }

        /// <summary>
        /// Checks if name is a valid project name
        /// </summary>
        /// <param name="name">the name to check</param>
        /// <returns>true if valid</returns>
        public bool IsValidProjectName(string name)
        {
            Project p = GetProjectList().Find(x => x.Name == name);
            return p == null;
        }
        #endregion

        #region Task Methods
        /// <summary>
        /// Gets the task by id
        ///   Date: 02/13/2021
        ///   Author: Robert Nelson
        /// </summary>
        /// <param name="id">task id</param>
        /// <returns>Task or null</returns>
        public Task GetTaskById(int id)
        {
            return GetTasks().Find(x => x.Id == id);
        }

        /// <summary>
        /// Creates the task
        ///   Date: 02/16/2021
        ///   Author: Robert Nelson
        /// </summary>
        /// <param name="name">Task name</param>
        /// <param name="start">start date</param>
        /// <param name="end">end date</param>
        /// <param name="description">task description</param>
        /// <param name="duration">likely duration estimate</param>
        /// <param name="maxDuration">maximum duration estimate</param>
        /// <param name="minDuration">minimum duration estimate</param>
        /// <returns>newly created task on success or null on failure</returns>
        public Task CreateTask(string name, DateTime start, DateTime? end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0)
        {
            Project project = GetProject();
            Task task = null;
            try
            {
                task = new Task(name, start, end, duration, maxDuration, minDuration, description, project: project);
            } catch (Exception) { }
            return task;
        }

        public void DeleteTask(Task t)
        {
            Task task = GetTaskById(t.Id);
            if (task != null)
            {
                task.Delete();
                
            }
        }
        public List<Task> GetTasks()
        {
            return GetProject().SortedTasks;
        }

        public HashSet<Task> GetTaskSet() => GetProject().Tasks;

        public bool IsValidTaskName(string name)
        {
            return name != "" && GetTasks().Find(x => x.Name == name) == null;
        }

        #endregion

        #region User Methods
        /// <summary>
        /// Used to create a user with just a name (unregistered)
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>null if it failed, user on success</returns>
        public User CreateUser(string name)
        {
            try
            {
                User user = new User(name);
                return user;
            } catch(IDBItem.DuplicateKeyError)
            { }
            return null;
        }

        /// <summary>
        /// Gets the user by name (if they exist) otherwise creates
        /// </summary>
        /// <param name="name">username</param>
        /// <returns>user</returns>
        public User CreateOrGetUser(string name)
        {
            User u = GetUsers().Find(x => x.Username == name);
            if (u == null)
                u = CreateUser(name);
            return u;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>user list</returns>
        public List<User> GetUsers() => reader.Users.Values.ToList();

        /// <summary>
        /// Logs the user in
        /// </summary>
        /// <param name="email">email or username</param>
        /// <param name="password">password</param>
        /// <returns>true on success</returns>
        public bool Login(string email, string password) => reader.Login(email, password);

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="username">username that is unique</param>
        /// <param name="email">unique email</param>
        /// <param name="password">password</param>
        /// <param name="name">actual name</param>
        /// <returns></returns>
        public bool Register(string username, string email, string password, string name) => reader.Register(username, password, name, email);

        /// <summary>
        /// Checks if user is logged in
        /// </summary>
        /// <returns>true if logged in</returns>
        public bool IsLoggedIn() => reader.CurrentUser != null;

        /// <summary>
        /// Determines if new username is valid
        /// </summary>
        /// <param name="name">username</param>
        /// <returns>True if valid</returns>
        public bool IsValidNewUsername(string name) => reader.IsValidUsername(name);

        /// <summary>
        /// Determines if new email is valid
        /// </summary>
        /// <param name="email">email address</param>
        /// <returns>True if valid</returns>
        public bool IsValidNewEmail(string email) => reader.IsValidEmail(email);

        /// <summary>
        /// Gets the current user that is logged in
        /// </summary>
        /// <returns>User or null</returns>
        public User GetCurrentUser() => reader == null ? null : reader.CurrentUser;
        #endregion

        #region Subscriber Methods
        public void Subscribe(IViewModel viewModel)
        {
            if (!viewModels.Contains(viewModel))
                viewModels.Add(viewModel);
        }

        public void UnSubscribe(IViewModel viewModel)
        {
            if (viewModels.Contains(viewModel))
                viewModels.Remove(viewModel);
        }
        #endregion

        #region Database Methods
        public void OnModelUpdate(Project p = null)
        {
            if(reader != null && !reader.IsUpdating && ! this.isUpdating)  // Don't send updates if we're in the middle of updating
            {
                isUpdating = true;
                if (p == null)
                    p = GetProject();
                foreach (IViewModel viewModel in viewModels)
                    viewModel.OnModelUpdate(p);
                isUpdating = false;
            }
        }

        public void OnDBUpdate(Project p = null) => OnModelUpdate(p);

        /// <summary>
        /// Refreshes the model
        /// </summary>
        public void Refresh()
        {
            reader.OnDBUpdate();
        }

        /// <summary>
        /// Determines if database is connected
        /// </summary>
        /// <returns>boolean</returns>
        public bool IsConnected()
        {
            return reader.Connected;
        }

        /// <summary>
        /// Sets the database connection string
        /// </summary>
        /// <param name="connectString">string</param>
        /// <returns>boolean</returns>
        public bool SetConnectionString(string connectString)
        {
            return this.reader.TestNewConnection(connectString);
        }

        /// <summary>
        /// Disconnected event
        /// </summary>
        public void OnDBDisconnect()
        {
            foreach (IViewModel viewModel in viewModels)
                viewModel.OnDisconnect();
        }


        #endregion

        #region Get Updated Version of objects
        public void UpdateProject(ref Project project, bool updateIfNull=true)
        {
            Project ret;
            if (reader.Projects.TryGetValue(project.Id, out ret))
                project = ret;
            else
            {
                foreach(Project p in reader.Projects.Values)
                    if(p.Name == project.Name)
                    {
                        project = p;
                        return;
                    }
                if (updateIfNull)
                    project = null;
            }
        }

        public void UpdateUser(ref User user, bool updateIfNull=true)
        {
            string uname = user.Username;
            User ret;
            if (reader.Users.TryGetValue(uname, out ret))
                user = ret;
            else if (updateIfNull)
                user = null;
        }

        public void UpdateTask(ref Task t, bool updateIfNull=true)
        {
            int id = t.Id;
            Task updated;
            if(!reader.Tasks.TryGetValue(id, out updated))
            {
                // Find by name
                string name = t.Name;
                foreach (Task task in reader.Tasks.Values)
                    if (task.Name == name)
                        updated = task;
            }
            if (updated != null || updateIfNull)
                t = updated;
        }

        #endregion
    }
}
