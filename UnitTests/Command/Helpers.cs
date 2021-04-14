using SmartPert.Command;
using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Command
{
    /// <summary>
    /// Some Helper classes for testing
    /// Created 4/4/2021 by Robert Nelson
    /// </summary>

    public class ModelClass
    {
        public int x;

        public ModelClass(int x)
        {
            this.x = x;
        }
    }

    public class AddOne : ICmd
    {
        private ModelClass model;

        public AddOne(ModelClass model)
        {
            this.model = model;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
        }

        public override void OnModelUpdate(Project p)
        {
        }

        public override bool Undo()
        {
            this.model.x = model.x - 1;
            return true;
        }

        protected override bool Execute()
        {
            this.model.x = model.x + 1;
            return true;
        }
    }
}
