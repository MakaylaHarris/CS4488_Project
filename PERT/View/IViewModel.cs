using PERT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERT.View
{
    public interface IViewModel
    {
        void OnModelUpdate(Project p);
    }
}
