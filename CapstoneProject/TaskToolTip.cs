using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CapstoneProject.Models;
using Task = CapstoneProject.Models.Task;

/// <summary>
/// Created by: Chris Neeser
/// Date: 10/20/2019
/// Purpose:  This class is used to override the tooltip class to have a custom tooltip on mouse hover of task.
/// </summary>

namespace CapstoneProject
{
    public class TaskToolTip: ToolTip
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(String), typeof(TaskToolTip),
             new PropertyMetadata("", HeaderPropertyChanged));

        public static readonly DependencyProperty TaskProperty =
            DependencyProperty.Register("Task", typeof(Task), typeof(TaskToolTip),
             new PropertyMetadata(new Task(), TaskPropertyChanged));

        private static void TaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void HeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public string Header { get => (string)this.GetValue(HeaderProperty); set => this.SetValue(HeaderProperty, value); }

        public Task Task { get => (Task)this.GetValue(TaskProperty); set => this.SetValue(TaskProperty, value); }

        public TaskToolTip()
        {

        }

        
    }
}
