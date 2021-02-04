using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pert.Model;

namespace Pert.Command
{
    /// <summary>
    /// Removes Task Dependency
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class RemoveDependencyCmd : ICmd
    {
        private readonly Task parent;
        private readonly Task dependent;

        public RemoveDependencyCmd(Task parent, Task dependent)
        {
            this.parent = parent;
            this.dependent = dependent;
        }

        public bool Execute()
        {
            parent.RemoveDependency(dependent);
            return true;
        }

        public bool Undo()
        {
            parent.AddDependency(dependent);
            return true;
        }
    }
}
