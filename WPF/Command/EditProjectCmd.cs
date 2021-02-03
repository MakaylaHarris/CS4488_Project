using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Model;

namespace WPF.Command
{
    class EditProjectCmd : ICmd
    {
        private readonly Project toEdit;
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
            old = new Project(toEdit.Name, toEdit.StartDate, toEdit.EndDate, toEdit.Description, toEdit.Id);
        }

        public bool Execute()
        {
            toEdit.Name = name;
            toEdit.StartDate = start;
            toEdit.EndDate = end;
            toEdit.Description = description;
            return true;
        }

        public bool Undo()
        {
            toEdit.Name = old.Name;
            toEdit.StartDate = old.StartDate;
            toEdit.EndDate = old.EndDate;
            toEdit.Description = old.Description;
            return true;
        }
    }
}
