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
    /// <summary>
    /// Interaction logic for HintAndDirect.xaml
    /// This is used for state transitions when the user needs to perform some action (login, connect to database, create project).
    /// Created 2/26/2021 by Robert Nelson
    /// </summary>
    public partial class HintAndLink : Page
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HintAndLink()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// The hint content (label)
        /// </summary>
        public string HintText { get; set; }

        /// <summary>
        /// The link content (button)
        /// </summary>
        public string LinkText { get; set; }

    }
}
