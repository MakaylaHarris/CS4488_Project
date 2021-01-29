using PERT.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.Model
{
    public class Model : IModel, DBUpdateReceiver
    {
        private IViewModel viewModel;
        private DBReader reader;
        public Model(IViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.reader = new DBReader(this);
        }

        public Project CreateProject(string name, DateTime start, DateTime end, string description = "")
        {
            throw new NotImplementedException();
        }

        public Task CreateTask(string name, DateTime start, DateTime end, string description = "", int duration = 1, int maxDuration = 0, int minDuration = 0)
        {
            throw new NotImplementedException();
        }

        public User CreateUser(string name)
        {
            throw new NotImplementedException();
        }

        public Project GetProject()
        {
            throw new NotImplementedException();
        }

        public List<Project> GetProjectList()
        {
            throw new NotImplementedException();
        }

        public List<Task> GetTasks()
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsers()
        {
            throw new NotImplementedException();
        }

        public void SetProject(Project project)
        {
            throw new NotImplementedException();
        }

        public void OnDBUpdate(Project p)
        {
            this.viewModel.OnModelUpdate(p);
        }
    }
}
