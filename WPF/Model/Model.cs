using System;
using System.Collections.Generic;
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
        private List<IViewModel> viewModels;
        private DBReader reader;

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
            reader = DBReader.Instantiate(this);
        }
        #endregion

        #region Project Methods
        public Project CreateProject(string name, DateTime start, DateTime? end, string description = "")
        {
            throw new NotImplementedException();
        }

        public void DeleteProject(Project p)
        {
            throw new NotImplementedException();
        }
        public Project GetProject()
        {
            return reader.CurrentProject;
        }
        public List<Project> GetProjectList()
        {
            return reader.Projects;
        }

        public void SetProject(Project project)
        {
            reader.SetProject(project);
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

        public Task CreateTask(string name, DateTime start, DateTime? end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0)
        {
            throw new NotImplementedException();
        }

        public void DeleteTask(Task t)
        {
            throw new NotImplementedException();
        }

        public List<Task> GetTasks()
        {
            return GetProject().Tasks;
        }

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
        public User CreateUser(string name) => reader.CreateUser(name);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>user list</returns>
        public List<User> GetUsers() => reader.Users;

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

        #endregion

        #region Subscriber Methods
        public void Subscribe(IViewModel viewModel)
        {
            if (!viewModels.Contains(viewModel))
                viewModels.Add(viewModel);
        }

        public void Unsubscribe(IViewModel viewModel)
        {
            if (viewModels.Contains(viewModel))
                viewModels.Remove(viewModel);
        }
        #endregion

        #region Database Methods
        public void OnModelUpdate(Project p = null)
        {
            if (p == null)
                p = GetProject();
            foreach (IViewModel viewModel in viewModels)
                viewModel.OnModelUpdate(p);
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

        #endregion
    }
}
