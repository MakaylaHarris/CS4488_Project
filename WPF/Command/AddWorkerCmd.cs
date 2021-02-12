using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
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

        protected override bool Execute()
        {
            item.AddWorker(worker);
            return true;
        }

        public override bool Undo()
        {
            item.RemoveWorker(worker);
            return true;
        }
    }
}
