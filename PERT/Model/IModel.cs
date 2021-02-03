using System;
using System.Collections.Generic;

namespace PERT.Model
{
    /// <summary>
    /// Interface that talks with IViewModel
    /// </summary>
    interface IModel
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

        bool Login(string email, string password);

        bool Register(string name, string email, string password);
        #endregion

        void Refresh();
    }
}
