using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.Command
{
    /// <summary>
    /// Command Stack singleton that maintains the Stack of commands executed
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class CommandStack
    {
        private static readonly CommandStack instance = new CommandStack();
        private Stack<ICmd> cmds;
        private Stack<ICmd> redoStack;

        static CommandStack() { }
        private CommandStack()
        {
            cmds = new Stack<ICmd>();
            redoStack = new Stack<ICmd>();
        }

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static CommandStack Instance
        {
            get { return instance; }
        }

        public void PushCommand(ICmd cmd)
        {
            if (redoStack.Count > 0)
                redoStack.Clear();
            cmds.Push(cmd);
        }

        #region Public methods

        public void Clear()
        {
            cmds.Clear();
            redoStack.Clear();
        }

        public bool CanUndo()
        {
            return cmds.Count > 0;
        }

        public bool Undo()
        {
            if (CanUndo())
            {
                ICmd cmd = cmds.Pop();
                if (cmd.Undo())
                {
                    redoStack.Push(cmd);
                    return true;
                }
                else   // uh oh! something went wrong so we're in a volatile state... don't do anything else
                {
                    cmds.Clear();
                    redoStack.Clear();
                }
            }
            return false;
        }

        public bool CanRedo()
        {
            return redoStack.Count > 0;
        }

        public bool Redo()
        {
            if (CanRedo())
            {
                return redoStack.Pop().Run();
            }
            return false;
        }
        #endregion
    }
}
