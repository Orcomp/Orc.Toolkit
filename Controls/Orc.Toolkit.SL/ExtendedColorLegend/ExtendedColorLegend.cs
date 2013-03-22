// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedColorLegend.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Commands for ExtendedColorLegend control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using Commands;

    /// <summary>
    /// Control to show color legend with checkboxes for each color
    /// </summary>
    [TemplatePart(Name = "PART_List", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_UnselectAll", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_All_Visible", Type = typeof(CheckBox))]
    public class ExtendedColorLegend : HeaderedContentControl
    {
        #region Dependency properties
        /// <summary>
        /// The operation color attribute property.
        /// </summary>
        public static readonly DependencyProperty OperationColorAttributeProperty =
            DependencyProperty.Register("OperationColorAttribute", typeof(string), typeof(ExtendedColorLegend), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Property for colors list
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable<IColorProvider>), typeof(ExtendedColorLegend), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// The is all visible property.
        /// </summary>
        public static readonly DependencyProperty IsAllVisibleProperty = DependencyProperty.Register(
            "IsAllVisible", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(false));

        /// <summary>
        /// Property for colors list
        /// </summary>
        public static readonly DependencyProperty FilteredItemsSourceProperty = DependencyProperty.Register("FilteredItemsSource", typeof(IEnumerable<IColorProvider>), typeof(ExtendedColorLegend), new PropertyMetadata(null));

        /// <summary>
        /// The selected items property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
            "SelectedColorItems",
            typeof(IEnumerable<IColorProvider>),
            typeof(ExtendedColorLegend),
            new PropertyMetadata(null));

        /// <summary>
        /// Property indicating whether color can be edited or not
        /// </summary>
        public static readonly DependencyProperty AllowColorEditingProperty = DependencyProperty.Register("AllowColorEditing", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(true));

        /// <summary>
        /// The is drop down open property.
        /// </summary>
        public static readonly DependencyProperty IsColorSelectingProperty = DependencyProperty.Register(
            "IsColorSelecting",
            typeof(bool),
            typeof(ExtendedColorLegend),
            new PropertyMetadata(false, OnIsColorSelectingPropertyChanged));

        /// <summary>
        /// Property indicating whether search is performing using regex or not
        /// </summary>
        public static readonly DependencyProperty UseRegexFilteringProperty = DependencyProperty.Register("UseRegexFiltering", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(true));

        /// <summary>
        /// The current color property.
        /// </summary>
        public static readonly DependencyProperty EditingColorProperty = DependencyProperty.Register(
            "EditingColor", typeof(Color), typeof(ExtendedColorLegend), new PropertyMetadata(Colors.White));

#if(!SILVERLIGHT)
        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowSearchBoxProperty = DependencyProperty.Register("ShowSearchBox", typeof(bool), typeof(ExtendedColorLegend), new UIPropertyMetadata(true));

        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowToolBoxProperty = DependencyProperty.Register("ShowToolBox", typeof(bool), typeof(ExtendedColorLegend), new UIPropertyMetadata(true));

        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowColorVisibilityControlsProperty = DependencyProperty.Register("ShowColorVisibilityControls", typeof(bool), typeof(ExtendedColorLegend), new UIPropertyMetadata(true));

        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowSettingsProperty = DependencyProperty.Register("ShowSettings", typeof(bool), typeof(ExtendedColorLegend), new UIPropertyMetadata(true));

        /// <summary>
        /// Expose filter property
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(ExtendedColorLegend), new UIPropertyMetadata(null, OnFilterChanged));

        /// <summary>
        /// Property for filter watermark
        /// </summary>
        public static readonly DependencyProperty FilterWatermarkProperty = DependencyProperty.Register("FilterWatermark", typeof(string), typeof(ExtendedColorLegend), new UIPropertyMetadata("Search"));

        #else
        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowSearchBoxProperty = DependencyProperty.Register("ShowSearchBox", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(true));

        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowToolBoxProperty = DependencyProperty.Register("ShowToolBox", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(true));

        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowColorVisibilityControlsProperty = DependencyProperty.Register("ShowColorVisibilityControls", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(true));

        /// <summary>
        /// Property indicating whether search box is shown or not
        /// </summary>
        public static readonly DependencyProperty ShowSettingsProperty = DependencyProperty.Register("ShowSettingsBox", typeof(bool), typeof(ExtendedColorLegend), new PropertyMetadata(true));

        /// <summary>
        /// Expose filter property
        /// </summary>
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register("Filter", typeof(string), typeof(ExtendedColorLegend), new PropertyMetadata(null, OnFilterChanged));

        /// <summary>
        /// Property for filter watermark
        /// </summary>
        public static readonly DependencyProperty FilterWatermarkProperty = DependencyProperty.Register("FilterWatermark", typeof(string), typeof(ExtendedColorLegend), new PropertyMetadata("Search"));
#endif
        #endregion

        /// <summary>
        /// The color board.
        /// </summary>
        private ColorBoard colorBoard;

        /// <summary>
        /// The listbox.
        /// </summary>
        private ListBox listBox;

        /// <summary>
        /// The popup
        /// </summary>
        private Popup popup;

            /// <summary>
        /// The button color change
        /// </summary>
        private ButtonBase button;

        public CheckBox checkBox;

        /// <summary>
        /// Item color of which is editing now
        /// </summary>
        private IColorProvider currentColorProvider;

        /// <summary>
        /// Change color command
        /// </summary>
        private ICommand changeColorCommand;        

        /// <summary>
        /// Initializes static members of the <see cref="ExtendedColorLegend" /> class.
        /// </summary>
        static ExtendedColorLegend()
        {
            #if (!SILVERLIGHT)
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedColorLegend), new FrameworkPropertyMetadata(typeof(ExtendedColorLegend)));
            #endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedColorLegend" /> class.
        /// </summary>
        public ExtendedColorLegend()
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
        /// Gets or sets the operation color attribute.
        /// </summary>
        public string OperationColorAttribute
        {
            get { return (string)GetValue(OperationColorAttributeProperty); }
            set { this.SetValue(OperationColorAttributeProperty, value); }
        }

        #if (SILVERLIGHT)
        /// <summary>
        ///     Gets or sets BrowseCommand.
        /// </summary>
        public ICommand ClearFilterCommand { get; set; }
        #endif

        /// <summary>
        /// Gets or sets a value indicating whether color can be edited or not
        /// </summary>
        public bool AllowColorEditing
        {
            get
            {
                return (bool)GetValue(AllowColorEditingProperty);
            }

            set
            {
                this.SetValue(AllowColorEditingProperty, value);
            }
        }

        
        /// <summary>
        /// Gets or sets a value indicating whether search box is shown or not
        /// </summary>
        public bool ShowSearchBox
        {
            get
            {
                return (bool)GetValue(ShowSearchBoxProperty);
            }

            set
            {
                this.SetValue(ShowSearchBoxProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tool box is shown or not
        /// </summary>
        public bool ShowToolBox
        {
            get
            {
                return (bool)GetValue(ShowToolBoxProperty);
            }

            set
            {
                this.SetValue(ShowToolBoxProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether settings button is shown or not
        /// </summary>
        public bool ShowSettings
        {
            get
            {
                return (bool)GetValue(ShowSettingsProperty);
            }

            set
            {
                this.SetValue(ShowSettingsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether settings button is shown or not
        /// </summary>
        public bool ShowColorVisibilityControls
        {
            get
            {
                return (bool)GetValue(ShowColorVisibilityControlsProperty);
            }

            set
            {
                this.SetValue(ShowColorVisibilityControlsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user editing current color
        /// </summary>
        public bool IsColorSelecting
        {
            get
            {
                return (bool)GetValue(IsColorSelectingProperty);
            }

            set
            {
                this.SetValue(IsColorSelectingProperty, value);
            }
        }

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
        /// Gets or sets a value indicating whether user editing current color
        /// </summary>
        public Color EditingColor
        {
            get
            {
                return (Color)GetValue(EditingColorProperty);
            }

            set
            {
                this.SetValue(EditingColorProperty, value);
                if (this.currentColorProvider != null)
                {
                    this.currentColorProvider.Color = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets filter for list of color
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
        /// Gets or sets source for color items
        /// </summary>
        public IEnumerable<IColorProvider> ItemsSource
        {
            get
            {
                return (IEnumerable<IColorProvider>)GetValue(ItemsSourceProperty);
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
                this.FilteredItemsSource = this.GetFilteredItems();
            }
        }

        public bool IsAllVisible
        {
            get
            {
                return (bool) GetValue(IsAllVisibleProperty);
            }

            set
            {
                this.SetValue(IsAllVisibleProperty, value);                
            }
        }

        /// <summary>
        /// Gets or sets a source for color items respecting current filter value
        /// </summary>
        public IEnumerable<IColorProvider> FilteredItemsSource
        {
            get
            {
                return (IEnumerable<IColorProvider>)GetValue(FilteredItemsSourceProperty);
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
        public IEnumerable<IColorProvider> SelectedColorItems
        {
            get
            {
                return (IEnumerable<IColorProvider>)this.GetValue(SelectedItemsProperty);
            }

            set
            {
                this.SetValue(SelectedItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color change command
        /// </summary>
        public ICommand ChangeColorCommand
        {
            get
            {
                return this.changeColorCommand ?? (this.changeColorCommand = new DelegateCommand(o =>
                {
                    if (!this.AllowColorEditing)
                    {
                        return;
                    }

                    this.currentColorProvider = null;
                    var colorProvider = o as IColorProvider;

                    if (colorProvider == null)
                    {
                        return;
                    }

                    this.EditingColor = colorProvider.Color;
                    this.currentColorProvider = colorProvider;
                    this.IsColorSelecting = true;

                }, p => p != null));
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
            this.popup = (Popup)this.GetTemplateChild("PART_Popup");
            this.button = (ButtonBase)GetTemplateChild("PART_UnselectAll");
            this.checkBox = (CheckBox)GetTemplateChild("PART_All_Visible");

            if (this.listBox != null)
            {
                this.listBox.SelectionChanged += (sender, args) => { this.SelectedColorItems = this.GetSelectedList(); };
                this.listBox.SelectionMode = SelectionMode.Extended;

                //#if (!SILVERLIGHT)
                //this.listBox.MouseDoubleClick += this.ListBoxMouseDoubleClick;
                //#endif
            }

            if (button != null)
            {
                button.Click += (s, e) =>
                    {
                        if (listBox != null)
                            listBox.SelectedIndex = -1;
                    };
            }

            if (this.checkBox != null) 
            {
                this.checkBox.Checked += (sender, args) => { 
                    this.IsAllVisible = true; 
                };
                this.checkBox.Unchecked += (sender, args) => { 
                    this.IsAllVisible = false; 
                };
            }

            this.colorBoard = new ColorBoard();
            if (this.popup != null)
            {
                this.popup.Child = this.colorBoard;
            }

            var b = new Binding("Color") { Mode = BindingMode.TwoWay, Source = this.colorBoard };

            this.SetBinding(EditingColorProperty, b);
            this.colorBoard.DoneClicked += this.ColorBoardDoneClicked;
            this.colorBoard.CancelClicked += this.ColorBoardCancelClicked;
        }

        /// <summary>
        /// The get selected list.
        /// </summary>
        /// <returns>
        /// The <see cref="ObservableCollection"/>.
        /// </returns>
        public IEnumerable<IColorProvider> GetSelectedList()
        {
            var result = new ObservableCollection<IColorProvider>();

            foreach (object item in this.listBox.SelectedItems)
            {
                result.Add(item as IColorProvider);
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
            var extendedColorLegend = o as ExtendedColorLegend;

            if (extendedColorLegend != null)
            {
                extendedColorLegend.ItemsSource = (IEnumerable<IColorProvider>)e.NewValue;
            }
        }

        /// <summary>
        /// Process filter changed event
        /// </summary>
        /// <param name="o">Sender dependency object</param>
        /// <param name="e">shows what has been changed</param>
        private static void OnFilterChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ExtendedColorLegend extendedColorLegend = o as ExtendedColorLegend;

            if (extendedColorLegend != null)
            {
                extendedColorLegend.OnFilterChanged((string)e.OldValue, (string)e.NewValue);
            }
        }

        /// <summary>
        /// The on is drop down open property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnIsColorSelectingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
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
        protected IEnumerable<IColorProvider> GetFilteredItems()
        {
            var items = (IEnumerable<IColorProvider>)GetValue(ItemsSourceProperty);
            var filter = (string)GetValue(FilterProperty);

            if ((items == null) || string.IsNullOrEmpty(filter))
            {
                return items;
            }

            try
            {
                Regex regex = this.UseRegexFiltering ? new Regex(filter) : new Regex(this.ConstructWildcardRegex(filter));
                return items.Where(cp => regex.IsMatch(cp.Description));
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

        #if (!SILVERLIGHT)
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

        /// <summary>
        /// Handler for double click
        /// </summary>
        /// <param name="sender">the list box item</param>
        /// <param name="e">event parameters</param>
        private void ListBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!this.AllowColorEditing)
            {
                return;
            }

            this.currentColorProvider = null;
            var elem = (UIElement)this.InputHitTest(e.GetPosition(this));
            ListBoxItem clickedItem = null;

            while (elem != this)
            {
                if (elem is ListBoxItem)
                {
                    clickedItem = (ListBoxItem)elem;
                    break;
                }

                elem = (UIElement)VisualTreeHelper.GetParent(elem);
            }

            if (clickedItem == null)
            {
                return;
            }

            var colorProvider = clickedItem.Content as IColorProvider;

            if (colorProvider == null)
            {
                return;
            }

            this.EditingColor = colorProvider.Color;
            this.currentColorProvider = colorProvider;
            this.IsColorSelecting = true;
        }

        #endif

        /// <summary>
        /// The color board_ done clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ColorBoardDoneClicked(object sender, RoutedEventArgs e)
        {
            this.EditingColor = this.colorBoard.Color;
            this.popup.IsOpen = false;
        }

        /// <summary>
        /// The color board_ cancel clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ColorBoardCancelClicked(object sender, RoutedEventArgs e)
        {
            this.popup.IsOpen = false;
        }
    }
}
