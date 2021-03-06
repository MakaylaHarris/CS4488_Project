﻿using SmartPert.View;
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
        bool IsValidProjectName(string name);
        Project GetProject();
        void SetProject(Project project);
        List<Project> GetProjectList();
        Project CreateProject(string name, DateTime start, DateTime? end, string description = "");

        void DeleteProject(Project project);
        #endregion

        #region Task Methods
        Task GetTaskById(int id);
        HashSet<Task> GetTaskSet();

        List<Task> GetTasks();

        Task CreateTask(string name, DateTime start, DateTime? end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0);
        void DeleteTask(Task task);

        bool IsValidTaskName(string name);
        #endregion

        #region User Methods
        List<User> GetUsers();
        User CreateUser(string name);
        User CreateOrGetUser(string name);
        User GetCurrentUser();

        bool IsValidNewUsername(string name);

        bool IsValidNewEmail(string email);

        bool IsLoggedIn();

        bool Login(string email, string password);

        bool Register(string username, string email, string password, string name);
        #endregion

        #region Observer Methods
        void Subscribe(IViewModel viewModel);

        void UnSubscribe(IViewModel viewModel);
        #endregion

        #region Database Methods
        bool IsConnected();

        bool SetConnectionString(string connectString);
        void Refresh();
        #endregion
    }
}
