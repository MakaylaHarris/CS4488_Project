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

namespace SmartPert.View.Controls
{
    /// <summary>
    /// Delegate function for integer change
    /// </summary>
    /// <param name="sender">The Integer Edit control</param>
    /// <param name="x">The integer value</param>
    public delegate void IntegerChangeDelegate(object sender, int x);

    /// <summary>
    /// Interaction logic for IntegerEdit.xaml
    /// Created 2/12/2021 by Robert Nelson
    /// </summary>
    public partial class IntegerEdit : UserControl
    {
        private int min;
        private int max;
        private int myInt;
        private IntegerChangeDelegate handler;

        #region Properties
        /// <summary>
        /// The current value
        /// </summary>
        public int Value
        {
            get => myInt;
            set
            {
                if (value != myInt)
                {
                    if (value >= min && value <= max)
                    {
                        myInt = value;
                        handler?.Invoke(this, myInt);
                    }
                    MyEdit.Text = myInt.ToString();
                }
            }
        }

        /// <summary>
        /// Minimum value of edit property
        /// </summary>
        public int Min { get => min;
            set
            {
                if (value > max)
                    throw new Exception("Min must be less than max!");
                min = value;
                if (value > myInt)
                    Value = min;
            }
        }

        /// <summary>
        /// Maximum value
        /// </summary>
        public int Max
        {
            get => max;
            set
            {
                if(value < min)
                    throw new Exception("Min must be less than max!");
                max = value;
                if (value < myInt)
                    Value = max;
            }
        }

        /// <summary>
        /// Integer Change delegate
        /// </summary>
        public IntegerChangeDelegate IntegerChange
        {
            get => handler;
            set => handler = value;
        }

        #endregion

        /// <summary>
        /// Constructor for integer edit control
        /// </summary>
        public IntegerEdit()
        {
            InitializeComponent();
            this.min = 1;
            this.max = 2000000000;
            Value = 1;
        }


        #region private methods
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            Value += 1;
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            Value -= 1;
        }

        private void MyEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = int.Parse(MyEdit.Text);
                Value = x;
            } catch
            {
                MyEdit.Text = Value.ToString();
            }
        }
        #endregion
    }
}
