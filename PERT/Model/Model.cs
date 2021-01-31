using PERT.View;
using System;
using System.Collections.Generic;

namespace PERT.Model
{
    /// <summary>
    /// The model is the pipeline between the view and the database
    /// Created 1/28/2021 by Robert Nelson
    /// </summary>
    public class Model : IModel, DBUpdateReceiver
    {
        private IViewModel viewModel;
        private DBReader reader;
        public Model(IViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.reader = new DBReader(this);
        }

        #region Project Methods
        public Project CreateProject(string name, DateTime start, DateTime end, string description = "")
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
        public Task CreateTask(string name, DateTime start, DateTime end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0)
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

        #endregion

        #region User Methods
        public User CreateUser(string name)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers()
        {
            return reader.Users;
        }

        public bool Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool Register(string name, string email, string password)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Database Methods
        public void OnDBUpdate(Project p)
        {
            this.viewModel.OnModelUpdate(p);
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
