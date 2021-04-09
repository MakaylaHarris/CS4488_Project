using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Adds dependency to task
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public class AddDependencyCmd : ICmd
    {
        private Task parent;
        private Task dependent;

        public AddDependencyCmd(Task parent, Task dependent)
        {
            this.parent = parent;
            this.dependent = dependent;
        }

        protected override bool Execute()
        {
            if(parent.CanAddDependency(dependent))
            {
                parent.AddDependency(dependent);
                return true;
            }
            return false;
        }

        public override bool Undo()
        {
            parent.RemoveDependency(dependent);
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == this.parent)
                parent = (Task) newItem;
            else if (old == this.dependent)
                dependent = (Task) newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTask(ref parent);
            UpdateTask(ref dependent);
        }
    }
}
