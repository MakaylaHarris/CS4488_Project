﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CalendarDay : UserControl
    {
        private DateTime date;
        public CalendarDay(DateTime date)
        {
            InitializeComponent();
            this.date = date;
            this.Day.Text = date.ToString("MMM\ndd");
        }
    }
}
