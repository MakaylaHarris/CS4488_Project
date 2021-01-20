/*
 * Created by Levi Delezene 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapstoneProject.DAL;
//added namespace-alankar
namespace CapstoneProject.Models {
    public enum Status {
        Not_Started = 0,
        In_Progress,
        Completed
    }

    public class Task {
        //These classes are using auto properties. I don't know how they'll work with the database stuff.
        //It's easy enough to change if they won't work

        public Task() {
            this.DependentTasks = new List<Task>();
        }

        public Task(string newName, DateTime newStart, int newDuration, Boolean newRootNode) {
            this.Name = newName;
            this.StartedDate = newStart;
            this.MinDuration = newDuration;
            this.DependentTasks = new List<Task>();
            this.RootNode = newRootNode;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double MinDuration { get; set; }
        public double MaxDuration { get; set; }
        public double MostLikelyDuration { get; set; }
        public int Priority { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? StartedDate { get; set; }
        public DateTime DeletedDate { get; set; }
        public Boolean RootNode { get; set; }

        //modified date created by alankar pokhrel
        public DateTime? ModifiedDate { get; set; }

        public List<Task> DependentTasks { get; set; }
        public User Owner { get; set; }
        public Status Status { get; set; }
        public int ProjectId { get; set; }

        public void AddDependentTask(Task t) {
            this.DependentTasks.Add(t);
        }

        public Task save() {
            OTask oTask = new OTask();
            ODependency oDependency = new ODependency();
            if (oTask.Get(Id) == null) {
                oTask.Insert(this);
    
                //Create Dependancies
                foreach (Task task in DependentTasks)
                {
                    oDependency.Insert(new Dependency(task.Id, this.Id));

                    //if the task is a dependency now, make sure to remove the root node flag
                    //if (task.RootNode == true)
                    //{
                    //    task.RootNode = false;
                    //    task.save();
                    //}
                }
            } else {
                oTask.Update(this);

                //Delete Dependancies
                oDependency.DeleteAllForDepOnTask(this.Id);

                //Create Dependancies
                foreach (Task task in DependentTasks)
                {
                    oDependency.Insert(new Dependency(task.Id, this.Id));

                    //If the task is a dependency now, make sure to remove the root node flag
                    //if (task.RootNode == true)
                    //{
                    //    task.RootNode = false;
                    //    task.save();
                    //}
                }
            }
            oTask.UpdateRootNodeFlagOnAllTasks();
            return this;
        }

        public override string ToString() {
            return $"{Name}";
        }
    }
}

