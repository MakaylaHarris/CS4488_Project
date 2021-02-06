﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

/// <summary>
/// Name space for the SmartPert WPF Application
/// </summary>
namespace Pert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Menu bar
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prints the main content area (whatever is shown).
        /// Created 2/2/2021 by Robert Nelson
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ExecutedRoutedEventArgs</param>
        private void Print_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            // Render the control as bitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)MainContent.ActualWidth, (int)MainContent.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(MainContent);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            MemoryStream stream = new MemoryStream();
            png.Save(stream);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = stream;
            bi.EndInit();

            // Then create the drawing
            var vis = new DrawingVisual();
            using (var dc = vis.RenderOpen())
            {
                dc.DrawImage(bi, new Rect { Width = bi.Width, Height = bi.Height });
            }
            // Finally print
            var pdialog = new PrintDialog();
            if (pdialog.ShowDialog() == true)
            {
                pdialog.PrintVisual(vis, "My Image");
            }
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Undo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Redo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Cut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Cut_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Copy_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Paste_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Paste_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddTask_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void AddTask_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DBConnect_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            InputTextBox.Text = Properties.Settings.Default.ConnectionString;
            InputBox.Visibility = Visibility.Visible;
        }

        private void DBString_Change(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
            string connect_string = InputTextBox.Text;
            // Todo: Test connection here and then set it in properties
        }

        private void DBConnect_Cancel(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
        }

        private void Options_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Additional_Help(object sender, ExecutedRoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"advice.txt");
            var lines = File.ReadAllLines(path);
            var randomLineNumber = random.Next(0, lines.Length - 1);
            int start = randomLineNumber;
            if(lines[start] == "")
            {
                start++;
            } else
            {
                while(lines[start - 1] != "")
                    start--;
            }
            int end = start;
            string help = lines[start];
            while(lines[++end] != "")
            {
                help += "\n" + lines[end];
            }
            MessageBox.Show(help);
        }

        private void Docs_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void About_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            string bit_size;
            if (IntPtr.Size == 8)
            {
                bit_size = "64-Bit";
            }
            else
            {
                bit_size = "32-Bit";
            }
            string version = "SmartPert " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string authors = "Authors: Dan, Kaden, Makayla, Robert, Tyler";
            string website = "https://github.com/MakaylaHarris/CS4488_Project";
            string about = version + " " + bit_size + "\n" + authors + "\n" + website;
            MessageBox.Show(about, version, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Refresh_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}