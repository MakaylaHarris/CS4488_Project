using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.Command
{
    /// <summary>
    /// Transaction command, to perform multiple commands as a single unit
    /// </summary>
    public class TransactionCmd : ICmd
    {
        private List<ICmd> cmds;
        int currentCmdId;

        public TransactionCmd()
        {
            cmds = new List<ICmd>();
            currentCmdId = 0;
        }

        public void Add(ICmd cmd)
        {
            cmds.Add(cmd);
        }

        public bool Run(ICmd cmd)
        {
            if (currentCmdId >= cmds.Count || cmds[currentCmdId] != cmd)
                throw new InvalidOperationException("Cmd " + cmd.ToString() + " was not expected during transaction");
            currentCmdId++;
            return cmd.Run(pushStack: false);
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            foreach (ICmd cmd in cmds)
                cmd.OnIdUpdate(old, newItem);
        }

        public override void OnModelUpdate(Project p)
        {
            foreach (ICmd cmd in cmds)
                cmd.OnModelUpdate(p);
        }

        public override bool Undo()
        {
            bool result = true;
            for (int i = currentCmdId - 1; i >= 0; i--)
                if (!cmds[i].Undo())
                    return false;
            return result;
        }

        protected override bool Execute()
        {
            for(; currentCmdId < cmds.Count; currentCmdId++)
                if (!cmds[currentCmdId].Run(pushStack: false))
                    return false;
            return true;
        }
    }
}
