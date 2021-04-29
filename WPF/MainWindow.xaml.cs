using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
﻿using SmartPert.View;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmartPert.Model;
using SmartPert.View.Login;
using SmartPert.View.Pages;
using System.Windows.Threading;
using SmartPert.View.Windows;
using MessageBox = System.Windows.MessageBox;
using PrintDialog = System.Windows.Controls.PrintDialog;


namespace SmartPert
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewModel
    {
        static private Random random = new Random();
        private Model.Model model;
        private WorkSpace workSpace;
        private ObservableCollection<MenuItemViewModel> items;
        public ObservableCollection<MenuItemViewModel> OpenItems { get => items; }


        public MainWindow()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.Dispatcher.UnhandledException += this.HandleException;
            InitializeComponent();
            items = new ObservableCollection<MenuItemViewModel>();
            DataContext = this;
            InitModel();
            StateSwitcher.Instance.Start(this);
            PopulateProjects();
        }

        public static readonly DependencyProperty IsLoggedInProp = DependencyProperty.Register("IsLoggedIn", typeof(Boolean), typeof(MainWindow));

        public bool IsLoggedIn
        {
            get { return (bool) GetValue(IsLoggedInProp); }
            set { SetValue(IsLoggedInProp, value); }
        }

        void HandleException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
#if !DEBUG
            ErrorCatch errorCatch = new ErrorCatch(args.Exception, ErrorCatchBackToApp);
            MainContent.Content = errorCatch;
            args.Handled = true;
#endif
        }

        void ErrorCatchBackToApp()
        {
            //this.MainContent.Content = chart;
            this.MainContent.Content = workSpace;
        }

        private void InitModel()
        {
            model = Model.Model.GetInstance(this);
        }

#region Menu bar
        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            StateSwitcher.Instance.OnProjectCreateOrEdit();
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_CanIfLoggedIn(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = StateSwitcher.Instance.IsLoggedIn;
        }

        private void CommandBinding_CanIfActiveProject(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = StateSwitcher.Instance.HasActiveProject;

        private void Open_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            FileMenu.IsSubmenuOpen = true;
            OpenMenu.IsSubmenuOpen = true;
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

        private void Exit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Command.CommandStack.Instance.CanUndo();
        }

        private void Undo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Command.CommandStack.Instance.Undo();
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Command.CommandStack.Instance.CanRedo();
        }

        private void Redo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Command.CommandStack.Instance.Redo();
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
            e.CanExecute = MainContent.Content != null && MainContent.Content.GetType() == typeof(WorkSpace);
        }

        private void AddTask_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StateSwitcher.Instance.OnTaskCreateOrEdit();
        }

        public void ShowDBConnectionSettings()
        {
            InputTextBox.Text = Properties.Settings.Default.ConnectionString;
            InputBox.Visibility = Visibility.Visible;
            if (model != null && model.IsConnected())
                UpdateDBStatus("Connected", (SolidColorBrush)FindResource("MaterialDesignBody"));
            else
                UpdateDBStatus("Disconnected", Brushes.Red);

        }

        private void DBConnect_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            ShowDBConnectionSettings();
        }

        private void UpdateDBStatus(string status, Brush brush)
        {
            DBStatus.Text = status;
            DBStatus.Foreground = brush;
        }


        private void DBString_Change(object sender, RoutedEventArgs e)
        {
            string connect_string = InputTextBox.Text;
            UpdateDBStatus("Connecting...", Brushes.Yellow);
            if (model.SetConnectionString(connect_string))
            {
                UpdateDBStatus("Connected", Brushes.Green);
                InputBox.Visibility = Visibility.Collapsed;
                StateSwitcher.Instance.TryLogin();
            }
            else
                UpdateDBStatus("Connection failed!", Brushes.Red);
        }

        private void ConnectString_GetHelp(object sender, RoutedEventArgs e)
        {
            Process.Start("https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring?view=dotnet-plat-ext-5.0#remarks");
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
            Process.Start(Properties.Resources.Website);
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
            string website = Properties.Resources.Website;
            string about = version + " " + bit_size + "\n" + authors + "\n" + website;
            MessageBox.Show(about, version, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Refresh_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            model.Refresh();
        }

        private void Settings_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StateSwitcher.Instance.OnProjectCreateOrEdit(model.GetProject());
            ProjectCreator pc = new ProjectCreator();
        }
        
        private void Account_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StateSwitcher.Instance.OnAccountEdit();
        }

        private void Theme_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StateSwitcher.Instance.OnThemeEdit();
        }

        private void SignOut_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            StateSwitcher.Instance.OnSignout();
        }
        #endregion

        #region Model Update
        public void OnModelUpdate(Project p)
        {
            PopulateProjects();
        }

        private void PopulateProjects()
        {
            items.Clear();
            foreach (Project p in model.GetProjectList())
                items.Add(new MenuItemViewModel(p.Name, new OpenProjectCommand(model, p)));
        }

        public void OnDisconnect()
        {
        }
        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            model.Shutdown();
        }
    }
}
