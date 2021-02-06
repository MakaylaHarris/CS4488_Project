using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pert.Model;

namespace Pert.Command
{
    /// <summary>
    /// Add worker to a item
    /// Created 2/2/2021 by Robert Nelson
    /// </summary>
    class AddWorkerCmd : ICmd
    {
        private readonly TimedItem item;
        private readonly User worker;

        public AddWorkerCmd(TimedItem item, User worker)
        {
            this.item = item;
            this.worker = worker;
        }

        public bool Execute()
        {
            item.AddWorker(worker);
            return true;
        }

        public bool Undo()
        {
            item.RemoveWorker(worker);
            return true;
        }
    }
}
