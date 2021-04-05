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
        bool isRedo;

        public TransactionCmd()
        {
            cmds = new List<ICmd>();
            currentCmdId = 0;
        }

        public void Add(ICmd cmd, bool hasRun=false)
        {
            // We're trying to add new commands to an action that was already performed
            if (isRedo)
                return;
            cmds.Add(cmd);
            if (hasRun)
                currentCmdId++;
        }

        public bool Run(ICmd cmd)
        {
            if (currentCmdId >= cmds.Count || cmds[currentCmdId] != cmd)
            {
                if (isRedo)
                    return true; // ignore, we already have the commands we need to run
                else
                    throw new InvalidOperationException("Cmd " + cmd.ToString() + " was not expected during transaction");
            }
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
            isRedo = true;
            for (int i = currentCmdId - 1; i >= 0; i--)
            {
                if (!cmds[i].Undo())
                {
                    result = false;
                    break;
                }
                --currentCmdId;
            }
            return result;
        }

        protected override bool Execute()
        {
            for(; currentCmdId < cmds.Count; )
                if (!cmds[currentCmdId++].Run(pushStack: false))
                    return false;
            return true;
        }
    }
}
