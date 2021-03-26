using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.View.ViewClasses
{
    /// <summary>
    /// Utility class for storing project data to be outputted to a string. 
    /// Created 25 February 2021 by Tyler Kness-Miller
    /// </summary>
    class ToolTipProjectData
    {
        //Private fields. 
        private string name;
        private DateTime start;
        private DateTime? end;
        private string description;

        public ToolTipProjectData(string name, DateTime start, DateTime? end, string description = "")
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.description = description;
        }

        public string OutputToolTipProject()
        {
            string output =
                "Project Name: " + this.name + "\r\n" +
                "Description: " + this.description + "\r\n" +
                "Start Date: " + this.start.ToString() + "\r\n" +
                "End Date: " + this.end.ToString() + "\r\n";
            return output;
        }
    }
}
