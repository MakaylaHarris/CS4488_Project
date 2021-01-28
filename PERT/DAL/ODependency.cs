using PERT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PERT.DAL
{
    class ODependency : IQuery
    {
        private Task task;
        private Task dependent;
        public ODependency(Task task, Task dependent)
        {
            this.task = task;
            this.dependent = dependent;
        }

        public override void Delete()
        {
            throw new NotImplementedException();
        }

        public override void Insert()
        {
            throw new NotImplementedException();
        }

        public override void Select()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
 
    }
}
