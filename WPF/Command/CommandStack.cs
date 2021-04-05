using SmartPert.Model;
using SmartPert.View;
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
    public class CommandStack : IViewModel
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

        public Stack<ICmd> Cmds { get => cmds; }
        public Stack<ICmd> RedoStack { get => redoStack;}

        #region Command public methods
        // Used by commands only
        public void PushCommand(ICmd cmd, bool isRedo=false)
        {
            if (!isRedo && redoStack.Count > 0)
                redoStack.Clear();
            cmds.Push(cmd);
        }

        /// <summary>
        /// Updates an items id throughout the stack
        /// </summary>
        /// <param name="oldItem">old</param>
        /// <param name="newItem">new</param>
        public void UpdateIds(TimedItem oldItem, TimedItem newItem)
        {
            foreach (ICmd cmd in cmds)
                cmd.OnIdUpdate(oldItem, newItem);
            foreach (ICmd cmd in redoStack)
                cmd.OnIdUpdate(oldItem, newItem);
        }

        #endregion

        public void Clear()
        {
            cmds.Clear();
            redoStack.Clear();
        }

        #region Undo/Redo
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
                return redoStack.Pop().Run(isRedo: true);
            }
            return false;
        }

        public void OnModelUpdate(Project p)
        {
            foreach (ICmd cmd in cmds)
                cmd.OnModelUpdate(p);
            foreach (ICmd cmd in redoStack)
                cmd.OnModelUpdate(p);
        }

        public void OnDisconnect()
        {
            Clear();
        }
        #endregion
    }
}
