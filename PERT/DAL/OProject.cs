using PERT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.DAL
{
    class OProject : IQuery
    {
        private Project project;
        public OProject(Project p)
        {
            project = p;
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

        public static List<Project> SelectAll()
        {
            throw new NotImplementedException();
        }
    }
}
