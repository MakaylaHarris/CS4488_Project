using SmartPert.View;
using System;
using System.Collections.Generic;

namespace SmartPert.Model
{
    /// <summary>
    /// Interface that talks with IViewModel
    /// </summary>
    public interface IModel
    {
        #region Project Methods
        Project GetProject();
        void SetProject(Project project);
        List<Project> GetProjectList();
        Project CreateProject(string name, DateTime start, DateTime? end, string description = "");

        void DeleteProject(Project project);
        #endregion

        #region Task Methods
        List<Task> GetTasks();
        Task CreateTask(string name, DateTime start, DateTime? end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0);
        void DeleteTask(Task task);
        #endregion

        #region User Methods
        List<User> GetUsers();
        User CreateUser(string name);

        bool IsValidNewUsername(string name);

        bool IsValidNewEmail(string email);

        bool IsLoggedIn();

        bool Login(string email, string password);

        bool Register(string username, string email, string password, string name);
        #endregion

        #region Observer Methods
        void Subscribe(IViewModel viewModel);

        void Unsubscribe(IViewModel viewModel);
        #endregion

        #region Database Methods
        bool IsConnected();

        bool SetConnectionString(string connectString);
        void Refresh();
        #endregion
    }
}
