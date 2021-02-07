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
    public interface ICmd
    {
        bool Execute();
        bool Undo();
    }

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

        #region Public methods

        public void Clear()
        {
            cmds.Clear();
            redoStack.Clear();
        }

        public bool Execute(ICmd cmd)
        {
            bool result = cmd.Execute();
            if (result)
            {
                cmds.Push(cmd);
                redoStack.Clear();
            }
            return result;
        }

        public bool CanUndo()
        {
            return cmds.Count > 0;
        }

        public bool Undo()
        {
            if(CanUndo())
            {
                ICmd cmd = cmds.Pop();
                if(cmd.Undo())
                {
                    redoStack.Push(cmd);
                    return true;
                } else   // uh oh! something went wrong so we're in a volatile state... don't do anything else
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
            if(CanRedo())
            {
                ICmd cmd = redoStack.Pop();
                if (cmd.Execute())
                {
                    cmds.Push(cmd);
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
