using PERT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PERT.DAL
{
    class OTask : IQuery
    {
        private Task task;
        public OTask(Task task)
        {
            this.task = task;
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

        static public List<Task> SelectAll()
        {
            throw new NotImplementedException();
        }
    }
}
