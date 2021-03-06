﻿using System;
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
    public class RemoveWorkerCmd : ICmd
    {
        private TimedItem item;
        private User worker;

        public RemoveWorkerCmd(TimedItem item, User worker)
        {
            this.item = item;
            this.worker = worker;
        }

        protected override bool Execute()
        {
            item.RemoveWorker(worker);
            return true;
        }

        public override bool Undo()
        {
            item.AddWorker(worker);
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (old == item)
                item = newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateTimedItem(ref item);
            UpdateUser(ref worker);
        }
    }
}
