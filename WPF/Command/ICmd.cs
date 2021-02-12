using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.Command
{
    /// <summary>
    /// Command pattern interface, any command that can be undone should implement
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    public abstract class ICmd
    {
        /// <summary>
        /// Runs the command and adds it to the stack if successful
        /// </summary>
        /// <returns>True on success</returns>
        public bool Run()
        {
            if(Execute())
            {
                CommandStack.Instance.PushCommand(this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called by run to execute command
        /// </summary>
        /// <returns>true on success</returns>
        protected abstract bool Execute();

        /// <summary>
        ///  Undo the command
        /// </summary>
        /// <returns>true on success</returns>
        public abstract bool Undo();
    }

}
