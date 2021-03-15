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
using SmartPert.View.Login;

namespace SmartPert.View.Account
{
    /// <summary>
    /// Interaction logic for AccountInfo.xaml
    /// </summary>
    public partial class AccountInfo : Page
    {
        private AccountEditor window;

        public AccountInfo(AccountEditor window)
        {
            InitializeComponent();
            this.window = window;
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            window.SwitchToPassword();
        }
    }
}
