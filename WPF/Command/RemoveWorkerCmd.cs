using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
{
    /// <summary>
    /// Remove worker from item
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class RemoveWorkerCmd : ICmd
    {
        private readonly TimedItem item;
        private readonly User worker;

        public RemoveWorkerCmd(TimedItem item, User worker)
        {
            this.item = item;
            this.worker = worker;
        }

        public bool Execute()
        {
            item.RemoveWorker(worker);
            return true;
        }

        public bool Undo()
        {
            item.AddWorker(worker);
            return true;
        }
    }
}
