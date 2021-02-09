using SmartPert.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace SmartPert
{
    /// <summary>
    /// Created by: Chris Neeser
    /// Date: 10/20/2019
    /// Purpose:  This class is used to override the tooltip class to have a custom tooltip on mouse hover of task.
    /// Adapted for SmartPert 2/9/2021 by Robert Nelson
    /// </summary>
    public class TaskToolTip : ToolTip
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(String), typeof(TaskToolTip),
             new PropertyMetadata("", HeaderPropertyChanged));

        //public static readonly DependencyProperty TaskProperty =
        //    DependencyProperty.Register("Task", typeof(Task), typeof(TaskToolTip),
        //     new PropertyMetadata(new Task(), TaskPropertyChanged));
        private Task task;

        public TaskToolTip(Task t)
        {
            this.task = t;
        }

        private static void TaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void HeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public string Header { get => (string)this.GetValue(HeaderProperty); set => this.SetValue(HeaderProperty, value); }

        public Task Task { get => task; }

    }
}
