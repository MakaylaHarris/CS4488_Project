using SmartPert.Command;
using SmartPert.Model;
using SmartPert.View;
using SmartPert.View.Login;
using SmartPert.View.Pages;
using SmartPert.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SmartPert.View.Account;
using SmartPert.View.Theme;
using System.ComponentModel;

namespace SmartPert
{
    /// <summary>
    /// State Switcher Singleton that controls what is displayed
    /// Created 2/26/2021 by Robert Nelson
    /// </summary>
    public class StateSwitcher : IViewModel, INotifyPropertyChanged
    {
        private static readonly StateSwitcher instance = new StateSwitcher();
        private MainWindow main;
        private Window openDialog;
        private IModel model;
        private bool isConnected;
        private bool isLoggedIn;
        private bool hasActiveProject;

        #region Properties
        /// <summary>
        /// Get singleton instance
        /// </summary>
        static public StateSwitcher Instance { get => instance; }

        /// <summary>
        /// Main Window
        /// </summary>
        public MainWindow Main { get => main; }

        /// <summary>
        /// Is Database connected
        /// </summary>
        public bool IsConnected { get => isConnected; }

        /// <summary>
        /// Is logged in
        /// </summary>
        public bool IsLoggedIn { get => isLoggedIn; }

        /// <summary>
        /// Has an active project (and logged in)
        /// </summary>
        public bool HasActiveProject { get => hasActiveProject; }

        #endregion

        #region Start Switcher
        public void Start(MainWindow window)
        {
            main = window;
            model = Model.Model.GetInstance(this);
            Update(true);   
        }

        private StateSwitcher() { }
        #endregion

        #region Private Methods
        private void GetState()
        {
            isConnected = model.IsConnected();
            isLoggedIn = isConnected && model.IsLoggedIn();
            main.IsLoggedIn = isLoggedIn;
            main.IsNotLoggedIn = !isLoggedIn;
            hasActiveProject = isLoggedIn && model.GetProject() != null;
        }

        private void OpenDialog(Window window)
        {
            if (openDialog != null)
                openDialog.Close();
            openDialog = window;
            openDialog.ShowDialog();
            openDialog = null;
        }

        private void UpdateDialog()
        {
            if(!hasActiveProject)
            {
                if (openDialog.GetType() == typeof(TaskEditor))
                    openDialog.Close();
                else if (openDialog.GetType() == typeof(ProjectCreator))
                {
                    if (!isLoggedIn)
                        openDialog.Close();
                    else  // Logged in and no project
                    {
                        ProjectCreator projCreator = ((ProjectCreator)openDialog);
                        if (projCreator.Project != null)
                            projCreator.Project = null;
                    }
                }
            }
            // Can't log in if you're already logged in
            if (isLoggedIn && openDialog.GetType() == typeof(LoginWindow))
                openDialog.Close();
        }


        /// <summary>
        /// Updates main content depending on state
        /// </summary>
        private void UpdateMainContent()
        {
            HintAndLink hintAndLink;
            
            if (!isConnected)
            {
                hintAndLink = new HintAndLink() { HintText = "Connect to the server", LinkText = "Click here to connect!" };
                hintAndLink.HintLink.Click += OnDisconnect;
                main.MainContent.Content = hintAndLink;
            }
            else if (!isLoggedIn)
            {
                hintAndLink = new HintAndLink() { HintText = "", LinkText = "Login to continue!" };
                hintAndLink.HintLink.Click += TryLogin;
                main.MainContent.Content = hintAndLink;
            }
            else if (!hasActiveProject)
            {
                hintAndLink = new HintAndLink() { HintText = "Open (Ctrl+O) or create a project (Ctrl+N)", LinkText = "Click to create a project!" };
                hintAndLink.HintLink.Click += OnProjectCreate;
                main.MainContent.Content = hintAndLink;
            }
            else if(main.MainContent.Content.GetType() != typeof(WorkSpace))
                main.MainContent.Content = new WorkSpace();

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Edit or create task
        /// </summary>
        /// <param name="task">task</param>
        public void OnTaskCreateOrEdit(Model.Task task =null)
        {
            if (hasActiveProject)
                OpenDialog(new TaskEditor(task));
            else
                Update();
        }

        /// <summary>
        /// Routes to OnProjectCreateOrEdit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnProjectCreate(object sender, RoutedEventArgs e) => OnProjectCreateOrEdit();

        /// <summary>
        /// Edit or create project
        /// </summary>
        /// <param name="project">project</param>
        public void OnProjectCreateOrEdit(Project project = null)
        {
            if (!isConnected || !isLoggedIn)
                Update();
            else
                OpenDialog(new ProjectCreator() { Project = project });
        }


        /// <summary>
        /// Routes to OnDisconnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDisconnect(object sender, RoutedEventArgs e) => OnDisconnect();

        /// <summary>
        /// When database disconnects, show the database connection settings
        /// </summary>
        public void OnDisconnect()
        {
            CommandStack.Instance.OnDisconnect();
            Update();
            Main.ShowDBConnectionSettings();
        }

        /// <summary>
        /// Routes to TryLogin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TryLogin(object sender, RoutedEventArgs e) => TryLogin();

        /// <summary>
        /// When successfully connected, show login
        /// </summary>
        public void TryLogin()
        {
            OpenDialog(new LoginWindow(model));
            if (model.IsLoggedIn())
                Update();
        }

        /// <summary>
        /// Generic update
        /// </summary>
        /// <param name="hardUpdate">if set, this will also go to the appropriate screen to connect or login</param>
        public void Update(bool hardUpdate = false)
        {
            GetState();
            UpdateMainContent();
            if(openDialog != null)
                UpdateDialog();
            if(hardUpdate)
            {
                if (!isConnected)
                    Main.ShowDBConnectionSettings();
                else if (!isLoggedIn)
                    TryLogin();
            }
        }

        /// <summary>
        /// Model update
        /// </summary>
        /// <param name="p"></param>
        public void OnModelUpdate(Project p)
        {
            CommandStack.Instance.OnModelUpdate(p);
            Update();
        }

        public void OnAccountEdit()
        {
            if (!isConnected || !isLoggedIn)
                Update();
            else
                OpenDialog(new AccountEditor());
        }

        public void OnThemeEdit()
        {
            OpenDialog(new ThemeEditor());
        }

        public void OnSignout()
        {
            OpenDialog(new Signout());
        }
        #endregion

        #region INotifyPropertyChanged Members
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
