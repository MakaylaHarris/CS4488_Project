using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.Model
{
    class Project : TimedItem, IDBItem
    {
        private List<Task> tasks;

        public void AddTask(Task t)
        {

        }

        public void RemoveTask(Task t)
        {

        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Insert()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
