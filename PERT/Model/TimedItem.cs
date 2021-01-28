using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.Model
{
    abstract class TimedItem
    {
        private DateTime startDate;
        private DateTime endDate;
        private string name;
        private string description;
        private uint id;
        private bool isComplete;
        private List<User> workers;

        public void AddWorker(User worker)
        {

        }

        public void RemoveWorker(User worker)
        {

        }
    }
}
