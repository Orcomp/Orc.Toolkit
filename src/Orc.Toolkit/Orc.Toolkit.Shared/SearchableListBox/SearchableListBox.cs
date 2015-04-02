// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchableListBox.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Searchable List Box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Orc.Toolkit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Orc.Toolkit.Commands;

    /// <summary>
    /// The searchable list box.
    /// </summary>
    [TemplatePart(Name = "PART_List", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_UnselectAll", Type = typeof(ButtonBase))]
    public class SearchableListBox : HeaderedContentControl
    {
        #region Dependency properties

        /// <summary>
        /// The items source property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<string>), typeof(SearchableListBox), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// The filtered items source property.
        /// </summary>
        public static readonly DependencyProperty FilteredItemsSourceProperty = DependencyProperty.Register("FilteredItemsSource", typeof(IEnumerable<string>), typeof(SearchableListBox), new PropertyMetadata(null));

        /// <summary>
        /// The selected items property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IEnumerable<string>),
            typeof(SearchableListBox),
            new PropertyMetadata(null));

        /// <summary>
        /// Property indicating whether search is performing using regex or not
        /// </summary>
        public static readonly DependencyProperty UseRegexFilteringProperty = DependencyProperty.Register("UseRegexFiltering", typeof(bool), typeof(SearchableListBox), new PropertyMetadata(true));

        /// <summary>
        /// Expose filter property
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(SearchableListBox), new UIPropertyMetadata(null, OnFilterChanged));

        /// <summary>
        /// Property for filter watermark
        /// </summary>
        public static readonly DependencyProperty FilterWatermarkProperty = DependencyProperty.Register("FilterWatermark", typeof(string), typeof(SearchableListBox), new UIPropertyMetadata("Search"));
        #endregion

        /// <summary>
        /// The listbox.
        /// </summary>
        private ListBox listBox;

        /// <summary>
        /// The button.
        /// </summary>
        private ButtonBase button;

        /// <summary>
        /// Initializes static members of the <see cref="SearchableListBox"/> class.
        /// </summary>
        static SearchableListBox()
        {
            #if (!SILVERLIGHT)
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchableListBox), new FrameworkPropertyMetadata(typeof(SearchableListBox)));
            #endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchableListBox" /> class.
        /// </summary>
        public SearchableListBox()
        {
            #if (!SILVERLIGHT)
            CommandBindings.Add(
                new CommandBinding(ExtendedColorLegendCommands.ClearFilter, this.ClearFilter, this.CanClearFilter));
            #else
            this.ClearFilterCommand = new DelegateCommand(o => this.Filter = string.Empty, o => string.IsNullOrEmpty(this.Filter));
            #endif
        }

        #region Public properties
        /// <summary>
        /// Gets or sets a value indicating whether regex is used when search is performed
        /// </summary>
        public bool UseRegexFiltering
        {
            get
            {
                return (bool)GetValue(UseRegexFilteringProperty);
            }

            set
            {
                this.SetValue(UseRegexFilteringProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets filter for list of strings
        /// </summary>
        public string Filter
        {
            get
            {
                return (string)GetValue(FilterProperty);
            }

            set
            {
                this.SetValue(FilterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets source for string items
        /// </summary>
        public IEnumerable<string> ItemsSource
        {
            get
            {
                return (IEnumerable<string>)GetValue(ItemsSourceProperty);
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
                this.FilteredItemsSource = this.GetFilteredItems();
            }
        }

        /// <summary>
        /// Gets or sets a source for string items respecting current filter value
        /// </summary>
        public IEnumerable<string> FilteredItemsSource
        {
            get
            {
                return (IEnumerable<string>)GetValue(FilteredItemsSourceProperty);
            }

            set
            {
                this.SetValue(FilteredItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets filter watermark string we use in search textbox
        /// </summary>
        public string FilterWatermark
        {
            get
            {
                return (string)this.GetValue(FilterWatermarkProperty);
            }

            set
            {
                this.SetValue(FilterWatermarkProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets list of selected items
        /// </summary>
        public IEnumerable<string> SelectedItems
        {
            get
            {
                return (IEnumerable<string>)this.GetValue(SelectedItemsProperty);
            }

            set
            {
                this.SetValue(SelectedItemsProperty, value);
            }
        }
        #endregion

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.listBox = (ListBox)this.GetTemplateChild("PART_List");
            this.button = (ButtonBase)GetTemplateChild("PART_UnselectAll");

            if (this.listBox != null)
            {
                this.listBox.SelectionChanged += (sender, args) => { this.SelectedItems = this.GetSelectedList(); };
                this.listBox.SelectionMode = SelectionMode.Extended;

                //#if (!SILVERLIGHT)
                //this.listBox.MouseDoubleClick += this.ListBoxMouseDoubleClick;
                //#endif
            }

            if (this.button != null)
            {
                this.button.Click += (s, e) =>
                {
                    if (listBox != null)
                    {
                        this.listBox.SelectedIndex = -1;
                    }
                };
            }
        }

        /// <summary>
        /// The get selected list.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>ObservableCollection</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEnumerable<string> GetSelectedList()
        {
            var result = new ObservableCollection<string>();

            foreach (object item in this.listBox.SelectedItems)
            {
                result.Add(item as string);
            }

            return result;
        }

        /// <summary>
        /// handler for rebinding ItemsSource property
        /// </summary>
        /// <param name="o">the dependency object</param>
        /// <param name="e">event params</param>
        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var searchableListBox = o as SearchableListBox;

            if (searchableListBox != null)
            {
                searchableListBox.ItemsSource = (IEnumerable<string>)e.NewValue;
            }
        }

        /// <summary>
        /// Process filter changed event
        /// </summary>
        /// <param name="o">Sender dependency object</param>
        /// <param name="e">shows what has been changed</param>
        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SearchableListBox searchableListBox = o as SearchableListBox;

            if (searchableListBox != null)
            {
                searchableListBox.OnFilterChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        /// <summary>
        /// Construct wildcard regex for case when regex is disable
        /// </summary>
        /// <param name="pattern">the search string</param>
        /// <returns>regex pattern</returns>
        private string ConstructWildcardRegex(string pattern)
        {
            // Always add a wildcard at the end of the pattern
            pattern = pattern.TrimEnd('*') + "*";

            // Make it case insensitive by default
            return "(?i)^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
        }

        /// <summary>
        /// Gets items filtered by filter
        /// </summary>
        /// <returns>filtered items</returns>
        protected IEnumerable<string> GetFilteredItems()
        {
            var items = (IEnumerable<string>)GetValue(ItemsSourceProperty);
            var filter = (string)GetValue(FilterProperty);

            if ((items == null) || string.IsNullOrEmpty(filter))
            {
                return items;
            }

            try
            {
                Regex regex = this.UseRegexFiltering ? new Regex(filter) : new Regex(this.ConstructWildcardRegex(filter));
                return items.Where(cp => regex.IsMatch(cp));
            }
            catch (Exception)
            {
                return items;
            }
        }

        /// <summary>
        /// Process filter changed event
        /// </summary>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        protected virtual void OnFilterChanged(string oldValue, string newValue)
        {
            this.FilteredItemsSource = this.GetFilteredItems();
        }

        #region Commands

        /// <summary>
        /// Clears filter
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event params</param>
        private void ClearFilter(object sender, ExecutedRoutedEventArgs e)
        {
            this.Filter = string.Empty;
        }

        /// <summary>
        /// Shows if clear filter command can be executed
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event params</param>
        private void CanClearFilter(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(this.Filter);
        }

        #endregion //Commands

    }
}

#endif