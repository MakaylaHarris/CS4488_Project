using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartPert.Model;

namespace SmartPert.Command
{
    public class EditProjectCmd : ICmd
    {
        private Project toEdit;
        private readonly string name;
        private readonly DateTime start;
        private readonly DateTime? end;
        private readonly string description;
        private readonly Project old;

        public EditProjectCmd(Project toEdit, string name, DateTime start, DateTime? end, string description)
        {
            this.toEdit = toEdit;
            this.name = name;
            this.start = start;
            this.end = end;
            this.description = description;
            old = new Project(toEdit.Name, toEdit.StartDate, toEdit.EndDate, toEdit.Description, toEdit.Creator, toEdit.CreationDate, toEdit.Id);
        }

        protected override bool Execute()
        {
            toEdit.Name = name;
            toEdit.StartDate = start;
            toEdit.EndDate = end;
            toEdit.Description = description;
            return true;
        }

        public override bool Undo()
        {
            toEdit.Name = old.Name;
            toEdit.StartDate = old.StartDate;
            toEdit.EndDate = old.EndDate;
            toEdit.Description = old.Description;
            return true;
        }

        public override void OnIdUpdate(TimedItem old, TimedItem newItem)
        {
            if (toEdit == old)
                toEdit = (Project)newItem;
        }

        public override void OnModelUpdate(Project p)
        {
            UpdateProject(ref toEdit);
        }
    }
}
