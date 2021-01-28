using PERT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.DAL
{
    class OUser : IQuery
    {
        private User user;
        public OUser(User user)
        {
            this.user = user;
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

        static public List<User> SelectAll()
        {
            throw new NotImplementedException();
        }
    }
}
