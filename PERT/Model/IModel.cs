using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.Model
{
    interface IModel
    {
        Project GetProject();
        void SetProject(Project project);
        List<Project> GetProjectList();
        Project CreateProject(string name, DateTime start, DateTime end, string description = "");

        List<Task> GetTasks();
        Task CreateTask(string name, DateTime start, DateTime end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0);

        List<User> GetUsers();
        User CreateUser(string name);
    }
}
