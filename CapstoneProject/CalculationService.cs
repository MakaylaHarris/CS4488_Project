/*
 * Created by Levi Delezene 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CapstoneProject.Models
{
    class CalculationService
    {
        public Rect dateToChartCoordinate(DateTime chartStartTime, Nullable<DateTime> taskStartTime, double duration)
        {
            Rect rect = new Rect();
            double days = ((DateTime)taskStartTime - chartStartTime).TotalDays;
            rect.X  = days * Properties.Settings.Default.dayWidth;
            rect.Width = duration * Properties.Settings.Default.dayWidth;

            return rect;
        }

        DateTime calculatePredictedEndDate(Project project)
        {
            throw new NotImplementedException();
        }
        //Other calulation methods
    }
}