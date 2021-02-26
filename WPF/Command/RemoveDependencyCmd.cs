using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Removes Task Dependency
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public class RemoveDependencyCmd : ICmd
    {
        private Task parent;
        private Task dependent;

        public RemoveDependencyCmd(Task parent, Task dependent)
        {
            this.parent = parent;
            this.dependent = dependent;
        }

        protected override bool Execute()
        {
            parent.RemoveDependency(dependent);
            return true;
        }

        public override bool Undo()
        {
            parent.AddDependency(dependent);
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == parent)
                parent = (Task) newItem;
            if (old == dependent)
                dependent = (Task)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref parent);
            UpdateTask(ref dependent);
        }
    }
}
