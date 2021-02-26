using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPert.View.ViewClasses
{
    /// <summary>
    /// Utility class for storing task data to be outputted in a string. 
    /// Created 2/25/2021 by Tyler Kness-Miller
    /// </summary>
    class ToolTipData
    {
        //Private fields.
        private string name;
        private DateTime start;
        private DateTime? end;
        private int duration;
        private int maxDuration;
        private int minDuration;
        private string description;

        /// <summary>
        /// Constructor for the ToolTipData class.
        /// </summary>
        /// <param name="name">The name of the associated task.</param>
        /// <param name="start">The start date of the associated task.</param>
        /// <param name="end">The end date of the associated task.</param>
        /// <param name="duration">The duration of the associated task.</param>
        /// <param name="maxDuration">The maximum duration of the associated task.</param>
        /// <param name="minDuration">The minimum duration of the associated task.</param>
        /// <param name="description">The description of the associated task.</param>
        public ToolTipData(string name, DateTime start, DateTime? end, int duration, int maxDuration = 0, int minDuration = 0, string description = "")
        {
            this.name = name;
            this.start = start;
            this.end = end;
            this.duration = duration;
            this.maxDuration = maxDuration;
            this.minDuration = minDuration;
            this.description = description;
        }

        /// <summary>
        /// Method that takes all attributes stored and converts it into a string to display as a ToolTip.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string OutputToolTip()
        {
            string output = "Name: " + this.name + "\r\n" +
                "Description: " + this.description + "\r\n" +
                "Start Date: " + this.start.ToString() + "\r\n" +
                "End Date: " + this.end.ToString() + "\r\n" +
                "Duration: " + this.duration.ToString() + "\r\n" +
                "Max Duration: " + this.maxDuration.ToString() + "\r\n" +
                "Minimum Duration: " + this.minDuration.ToString() + "\r\n";
            return output;
        }
    }
}
