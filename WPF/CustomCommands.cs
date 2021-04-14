using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartPert
{
    /// <summary>
    /// Custom Commands with keyboard shortcuts used by main menu.
    /// Created 2/2/2021 by Robert Nelson and Kaden Marchetti
    /// </summary>
    public static class CustomCommands
    {
        public static readonly RoutedCommand AddTaskCommand = new RoutedUICommand("Add Task", "AddTaskCommand", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
{
            new KeyGesture(Key.T, ModifierKeys.Control)
        }));

        public static readonly RoutedCommand DBConnectCommand = new RoutedUICommand("Database Connection", "DBConnect", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.D, ModifierKeys.Control)
        }));

        public static readonly RoutedCommand OptionsCommand = new RoutedUICommand("Options", "OpenOptions", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.O, ModifierKeys.Alt)
        }));

        public static readonly RoutedCommand DocsCommand = new RoutedUICommand("Docs", "OpenDocs", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.F1)
        }));

        public static readonly RoutedCommand AboutCommand = new RoutedUICommand("About", "OpenAbout", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.F1, ModifierKeys.Alt)
        }));

        public static readonly RoutedCommand AddHelpCommand = new RoutedUICommand("Additional Help", "Additional Help", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.F1, ModifierKeys.Control)
        }));

        public static readonly RoutedCommand RefreshCommand = new RoutedUICommand("Refresh", "Refresh", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.F5)
        }));

        public static readonly RoutedCommand SettingsCommand = new RoutedUICommand("Settings", "Settings", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.S, ModifierKeys.Control)
        }));

        public static readonly RoutedCommand AcctSettingsCommand = new RoutedUICommand("Account", "Account", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.A,  ModifierKeys.Alt)
        }));

        public static readonly RoutedCommand ThemeCommand = new RoutedUICommand("Theme", "Theme", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.C,  ModifierKeys.Alt)
        }));
        public static readonly RoutedCommand SignOutCommand = new RoutedUICommand("Sign Out", "SignOut", typeof(MainWindow), new InputGestureCollection(new InputGesture[]
        {
            new KeyGesture(Key.L,  ModifierKeys.Alt)
        }));
    }
}
