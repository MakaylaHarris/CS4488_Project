using System;
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

namespace SmartPert.View.Pages
{
    public delegate void ErrBackHandler();

    /// <summary>
    /// Interaction logic for ErrorCatch.xaml
    /// </summary>
    public partial class ErrorCatch : Page
    {

        private ErrBackHandler backHandler;
        public ErrorCatch(Exception e, ErrBackHandler b)
        {
            InitializeComponent();
            ShowsNavigationUI = false;
            this.errLabel.Content = e.ToString();
            backHandler = b;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            backHandler();
        }
    }
}
