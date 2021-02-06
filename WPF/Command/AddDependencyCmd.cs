using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPF.Model;

namespace WPF.Command
{
    /// <summary>
    /// Adds dependency to task
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class AddDependencyCmd : ICmd
    {
        private readonly Task parent;
        private readonly Task dependent;

        public AddDependencyCmd(Task parent, Task dependent)
        {
            this.parent = parent;
            this.dependent = dependent;
        }

        public bool Execute()
        {
            parent.AddDependency(dependent);
            return true;
        }

        public bool Undo()
        {
            parent.RemoveDependency(dependent);
            return true;
        }
    }
}
