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
        private SelectionChanged selectionChanged;

        #region Properties
        /// <summary>
        /// Filtered items to select from in combobox
        /// </summary>
        public ObservableCollection<object> FilteredItems { get; set; }

        #region Selected Property
        /// <summary>
        /// Get the selected object
        /// </summary>
        public object Selected { get => GetValue(SelectedProperty); set => SetValue(SelectedProperty, value); }

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(object), typeof(FilteredComboBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedChange)));
        private static void OnSelectedChange(DependencyObject d, DependencyPropertyChangedEventArgs a) => ((FilteredComboBox)d).OnSelectedChange(a);
        private void OnSelectedChange(DependencyPropertyChangedEventArgs a)
        {
            selected = a.NewValue;
            cb.SelectedItem = selected;
        }
        #endregion

        #region CanAddItems Property
        /// <summary>
        /// If true, they can add their own items.
        /// </summary>
        public bool CanAddItems
        {
            get => (bool)GetValue(CanAddItemsProperty);
            set => SetValue(CanAddItemsProperty, value);
        }
        public static readonly DependencyProperty CanAddItemsProperty = DependencyProperty.Register("CanAddItems", typeof(bool), typeof(FilteredComboBox),
            new PropertyMetadata(true, new PropertyChangedCallback(OnCanAddItemsChanged)));

        private static void OnCanAddItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs a) => ((FilteredComboBox)d).OnCanAddItemsChanged(a);

        private void OnCanAddItemsChanged(DependencyPropertyChangedEventArgs a)
        {
            bool value = (bool)a.NewValue;
            CanAddButton.IsEnabled = value;
            CanAddButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion

        #region Items Property
        /// <summary>
        /// All Items
        /// </summary>
        public ObservableCollection<object> Items
        {
            get => GetValue(AllItemsProperty) as ObservableCollection<object>;
            set => SetValue(AllItemsProperty, value);
        }
        public static readonly DependencyProperty AllItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(FilteredComboBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnItemsChanged)));
        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs a) => ((FilteredComboBox)d).OnItemsChanged(a);
        private void OnItemsChanged(DependencyPropertyChangedEventArgs a)
        {
            allItems = a.NewValue as ObservableCollection<object>;
            filter(allItems, input, FilteredItems);
            cb.ItemsSource = FilteredItems;
        }
        #endregion

        /// <summary>
        /// Gets the user input
        /// </summary>
        public string Input { get => input; }

        #region SelectionChanged Property
        public SelectionChanged SelectionChanged { get => GetValue(SelectionChangeProperty) as SelectionChanged; set => SetValue(SelectionChangeProperty, value); }
        public static readonly DependencyProperty SelectionChangeProperty = DependencyProperty.Register("SelectionChanged", typeof(SelectionChanged), typeof(FilteredComboBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnSelectionChanged)));
        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs a) => ((FilteredComboBox)d).OnSelectionChanged(a);
        private void OnSelectionChanged(DependencyPropertyChangedEventArgs a)
        {
            selectionChanged = a.NewValue as SelectionChanged;
        }
        #endregion

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
            FilteredItems = new ObservableCollection<object>();
            SetValue(AllItemsProperty, new ObservableCollection<object>());
        }
        #endregion

        #region Private Methods
        private void stringFilter(ObservableCollection<object> items, string input, ObservableCollection<object> filtered)
        {
            filtered.Clear();
            foreach (object item in items)
            {
                String str = item.ToString();
                if (str.StartsWith(input))
                    filtered.Add(item);
            }
        }

        private void cb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = cb.Text + e.Text;
            if (input.Length > 0)
            {
                if (input.Length == 1)
                    cb.IsDropDownOpen = true;
                filter(allItems, input, FilteredItems);
                cb.ItemsSource = FilteredItems;
            }
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged(cb.SelectedItem, input);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectionChanged(null, cb.Text);
        }

        #endregion

    }
}
