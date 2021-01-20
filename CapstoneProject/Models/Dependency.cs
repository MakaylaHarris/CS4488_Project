/*
 * Created by Alankar Pokhrel 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProject.Models
{
    public class Dependency
    {
        public Dependency()
        {

        }
        public Dependency(int TaskId, int DepOnTaskId)
        {
            this.TaskId = TaskId;
            this.DepOnTaskId = DepOnTaskId;
        }

        public int DepOnTaskId { get; set; }
        public int TaskId { get; set; }
    }
}
