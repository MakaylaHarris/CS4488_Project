/*
 * Created by Levi Delezene 
 */

using CapstoneProject.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added namespace-alankar
namespace CapstoneProject.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public double WorkingHours { get; set; }

        //Project owner created by alankar Pokhrel
        public User ProjectOwner { get; set; }

        internal List<Task> Tasks { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public Project save() {
            OProject oProject = new OProject();
            if (oProject.Get(Id) == null) {
                oProject.Insert(this);
            } else {
                oProject.Update(this);
            }
            return this;
        }
    }
}