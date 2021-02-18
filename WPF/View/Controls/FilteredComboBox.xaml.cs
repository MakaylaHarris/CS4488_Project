using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Delegate filter function used to determine which items should be shown in combobox
    /// </summary>
    /// <param name="items">All the items</param>
    /// <param name="input">The user's input to use as filter</param>
    /// <param name="filtered">The filtered collection to be updated</param>
    /// <returns>List of filtered items</returns>
    public delegate void ItemFilter(ObservableCollection<object> items, string input, ObservableCollection<object> filtered);

    public delegate void SelectionChanged(object selected, string text);

    /// <summary>
    /// Interaction logic for FilteredComboBox.xaml
    /// Created 2/17/2021 by Robert Nelson
    /// </summary>
    public partial class FilteredComboBox : UserControl
    {
        private ObservableCollection<object> allItems;
        private ItemFilter filter;
        private string input;
        private object selected;

        #region Properties
        /// <summary>
        /// Filtered items to select from in combobox
        /// </summary>
        public ObservableCollection<object> FilteredItems { get; set; }

        /// <summary>
        /// Get the selected object
        /// </summary>
        public object Selected { get => selected; }

        /// <summary>
        /// All Items
        /// </summary>
        public ObservableCollection<object> Items
        {
            get => allItems;
            set
            {
                allItems = value;
                filter(allItems, input, FilteredItems);
                cb.ItemsSource = FilteredItems;
            }
        }

        /// <summary>
        /// Gets the user input
        /// </summary>
        public string Input { get => input; }

        public SelectionChanged SelectionChanged { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Filtered Combobox
        /// </summary>
        public FilteredComboBox()
        {
            InitializeComponent();
            this.filter = stringFilter;
            selected = null;
            input = "";
            allItems = new ObservableCollection<object>();
            FilteredItems = new ObservableCollection<object>();
        }
        #endregion

        #region Private Methods
        private void stringFilter(ObservableCollection<object> items, string input, ObservableCollection<object> filtered)
        {
            filtered.Clear();
            foreach(object item in items)
            {
                String str = item.ToString();
                if (str.StartsWith(input))
                    filtered.Add(str);
            }
        }

        private void cb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = e.Text;
            Console.WriteLine(input);
            filter(allItems, input, FilteredItems);
            cb.ItemsSource = FilteredItems;
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = cb.SelectedItem.ToString();
            object found = null;
            foreach(object item in allItems)
            {
                if(item.ToString() == selected)
                {
                    found = item;
                    break;
                }
            }
            SelectionChanged(found, input);
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectionChanged(null, cb.Text);
        }
    }
}
