using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Orc.Toolkit
{

    [TemplatePart(Name = "PART_ButtonsPresenter", Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(DataGrid))]
    [TemplatePart(Name = "PART_FooterTrayPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_ResizeGripper", Type = typeof(FrameworkElement))]
    public class AttributeConfigurationEditor : ItemsControl
    {
        private DataGrid dataGrid;
        private ItemsPresenter buttonsItemsPresenter;
        private Panel footerTrayPanel;
        private FrameworkElement resizeGripper;
        private bool isResizing = false;
        private Point lastPoint;
        private double buttonsPresenterInitialHeight;
        private List<AttributeConfigurationButton> buttons;

        public AttributeConfigurationEditor()
        {
            DefaultStyleKey = typeof(AttributeConfigurationEditor);
            this.buttons = new List<AttributeConfigurationButton>();
        }
        
#if(SILVERLIGHT)
        public static Visibility GetColumnVisibility(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(ColumnVisibilityProperty);
        }
        public static void SetColumnVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(ColumnVisibilityProperty, value);
        }
        public static readonly DependencyProperty ColumnVisibilityProperty =
            DependencyProperty.RegisterAttached("ColumnVisibility", typeof(Visibility), typeof(ExtendedDataGridColumn), new PropertyMetadata(Visibility.Visible,
                new PropertyChangedCallback(OnColumnVisibilityChanged)));
        private static void OnColumnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGridColumn = d as DataGridColumn;
            if (dataGridColumn == null)
            {
                return;
            }
            if (dataGridColumn.Visibility != (Visibility)e.NewValue)
            {
                dataGridColumn.Visibility = (Visibility)e.NewValue;
            }
        }
#endif

        public new IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly new DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AttributeConfigurationEditor), 
            new PropertyMetadata(null));
        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.footerTrayPanel = (Panel)GetTemplateChild("PART_FooterTrayPanel");
            this.buttonsItemsPresenter = (ItemsPresenter)GetTemplateChild("PART_ButtonsPresenter");
            this.dataGrid = (DataGrid)GetTemplateChild("PART_DataGrid");
            this.resizeGripper = (FrameworkElement)GetTemplateChild("PART_ResizeGripper");

            if (this.dataGrid == null)
            {
                throw new Exception("ControlTemplate does not contain PART_DataGrid!");
            }
            if (this.footerTrayPanel == null)
            {
                throw new Exception("ControlTemplate does not contain PART_FooterTrayPanel!");
            }
            if (this.buttonsItemsPresenter == null)
            {
                throw new Exception("ControlTemplate does not contain PART_ButtonsPresenter!");
            }
            if (this.resizeGripper == null)
            {
                throw new Exception("ControlTemplate does not contain PART_ResizeGripper!");
            }

            this.dataGrid.CanUserReorderColumns = true;
            this.dataGrid.CanUserResizeColumns = true;
            this.dataGrid.CanUserSortColumns = true;
            this.dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;            
            this.dataGrid.AutoGenerateColumns = false;
#if(!SILVERLIGHT)
            this.dataGrid.CanUserAddRows = false;
#endif
            var itemsSourceBinding = new Binding("ItemsSource") { Source = this };
            this.dataGrid.SetBinding(DataGrid.ItemsSourceProperty, itemsSourceBinding);

            this.resizeGripper.MouseLeftButtonDown += this.resizeGripper_MouseLeftButtonDown;
            this.resizeGripper.MouseMove += this.resizeGripper_MouseMove;
            this.resizeGripper.MouseLeftButtonUp += this.resizeGripper_MouseLeftButtonUp;

            this.buttonsItemsPresenter.SizeChanged += this.buttonsItemsPresenter_SizeChanged;
        }

        void buttonsItemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdateButtons();
        }
                       

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return false;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var attributeConfigurationButton = new AttributeConfigurationButton();
            return attributeConfigurationButton;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)      {

            var attributeConfigurationButton = (AttributeConfigurationButton)element;
            this.buttons.Add(attributeConfigurationButton);
            attributeConfigurationButton.MouseLeftButtonDown += this.AttributeConfigurationButton_MouseLeftButtonDown;

            if (attributeConfigurationButton != null && item is AttributeConfigurationEditorSection)
            {
                var attributeConfigurationEditorSection = (AttributeConfigurationEditorSection)item;
                base.PrepareContainerForItemOverride(attributeConfigurationButton, attributeConfigurationEditorSection);

                Binding sectionHeaderBinding = new Binding("Header") { Source = attributeConfigurationEditorSection };
                attributeConfigurationButton.SetBinding(AttributeConfigurationButton.ContentProperty, sectionHeaderBinding);

                Binding sectionHeaderImageBinding = new Binding("HeaderImage") { Source = attributeConfigurationEditorSection };
                attributeConfigurationButton.SetBinding(AttributeConfigurationButton.HeaderProperty, sectionHeaderImageBinding);

                foreach (var attributeConfigurationProperty in attributeConfigurationEditorSection.Properties)
                {
                    var dataGridColumn = new ExtendedDataGridColumn() 
                    {
                        DataGrid = this.dataGrid,
                        Header = attributeConfigurationProperty.Header,
                        BindingPath = attributeConfigurationProperty.BindingPath,
                        SupportedValuesDisplayMemberPath = attributeConfigurationProperty.SupportedValuesDisplayMemberPath,
                        SupportedValuesValuePath = attributeConfigurationProperty.SupportedValuesValuePath
#if(!SILVERLIGHT)
                        ,
                        CanUserSort = true,
                        SortMemberPath = attributeConfigurationProperty.BindingPath
#endif
                    };
                    var supportedValuesbinding = new Binding("SupportedValues") { Source = attributeConfigurationProperty, Mode = BindingMode.TwoWay };
                    BindingOperations.SetBinding(dataGridColumn, ExtendedDataGridColumn.SupportedValuesProperty, supportedValuesbinding);

                    if (!attributeConfigurationProperty.ShowInAllSections)
                    {                     
                        var visibilityBinding = new Binding("IsSelected");
                        visibilityBinding.Converter = new BoolToVisibilityConverter();
                        visibilityBinding.Source = attributeConfigurationButton;
#if(!SILVERLIGHT)   
                        BindingOperations.SetBinding(dataGridColumn, DataGridColumn.VisibilityProperty, visibilityBinding);
#else
                        BindingOperations.SetBinding(dataGridColumn, AttributeConfigurationEditor.ColumnVisibilityProperty, visibilityBinding);
#endif
                    }
                    dataGrid.Columns.Add(dataGridColumn);
                }
            }           
            this.UpdateButtons();
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            var attributeConfigurationEditorSection = (AttributeConfigurationEditorSection)item;
            var attributeConfigurationButton = (AttributeConfigurationButton)element;

            attributeConfigurationButton.ClearValue(AttributeConfigurationButton.ContentProperty);
            attributeConfigurationButton.ClearValue(AttributeConfigurationButton.HeaderProperty);

            if (attributeConfigurationButton.IsSelected)
            {
                attributeConfigurationButton.IsSelected = false;
            }
            this.buttons.Remove(attributeConfigurationButton);

            List<AttributeConfigurationButton> buttonsToRemove = new List<AttributeConfigurationButton>();
            foreach (AttributeConfigurationButton button in footerTrayPanel.Children)
            {
                if (button.Tag == attributeConfigurationButton)
                {
                    button.MouseLeftButtonDown -= this.AttributeConfigurationButton_MouseLeftButtonDown;
                }
            }
            foreach (var button in buttonsToRemove)
            {           
                this.footerTrayPanel.Children.Remove(button);
            }
        }

        void resizeGripper_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.isResizing) { return; }
            if (this.isResizing = this.resizeGripper.CaptureMouse())
            {
                this.lastPoint = e.GetPosition(this);
                this.buttonsPresenterInitialHeight = buttonsItemsPresenter.ActualHeight;
            }
        }

        void resizeGripper_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isResizing = false;
            this.resizeGripper.ReleaseMouseCapture();
        }

        void resizeGripper_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isResizing)
            {
                Point currentPoint = e.GetPosition(this);
                var delta = currentPoint.Y - lastPoint.Y;
                var y = (int)Math.Round(delta / 21.0);
                if (this.buttonsPresenterInitialHeight - y * 21 >= 0 && this.buttonsPresenterInitialHeight - y * 21 <= this.Items.Count * 21)
                {
                    this.buttonsItemsPresenter.Height = buttonsPresenterInitialHeight - y * 21;
                }  
            }
        }

        private void UpdateButtons()
        {
            int visibleButtonsCount = this.GetButtonsInView();
            int totalButtons = this.buttons.Count;
            int hiddenButtonsCount = totalButtons - visibleButtonsCount;
            this.footerTrayPanel.Children.Clear();
            for (int i = hiddenButtonsCount; i > 0; i--)
            {
                AttributeConfigurationButton button = this.buttons[totalButtons - i] as AttributeConfigurationButton;
                this.AddToFooterTray(button);
            }
        }

        private void AddToFooterTray(AttributeConfigurationButton button)
        {
            AttributeConfigurationButton footerTrayButton = new AttributeConfigurationButton();
            Image image = new Image();
            if (button.Header is Image)
            {
                image.Source = (button.Header as Image).Source;
            }
            footerTrayButton.Header = image;
            footerTrayButton.Content = button.Content;
            footerTrayButton.Tag = button;
            footerTrayButton.MouseLeftButtonDown += AttributeConfigurationButton_MouseLeftButtonDown;
            var isSelectedbinding = new Binding("IsSelected") { Source = button, Mode = BindingMode.TwoWay };
            footerTrayButton.SetBinding(AttributeConfigurationButton.IsSelectedProperty, isSelectedbinding);

            this.footerTrayPanel.Children.Add(footerTrayButton);
        }

        void AttributeConfigurationButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as AttributeConfigurationButton).Tag is AttributeConfigurationButton)
            {
                this.SelectItem((sender as AttributeConfigurationButton).Tag as AttributeConfigurationButton);
            }
            else
            {
                this.SelectItem((sender as AttributeConfigurationButton));
            }
        }

        private void SelectItem(AttributeConfigurationButton attributeConfigurationButton)
        {
            foreach (var button in buttons)
            {
                if (button == attributeConfigurationButton)
                {
                    if (!button.IsSelected)
                    {
                        button.IsSelected = true;
                    }
                }
                else
                {
                    if (button.IsSelected)
                    {
                        button.IsSelected = false;
                    }
                }
            }
        }

        private int GetButtonsInView()
        {
            double buttonHeight = 21;
            return (int)Math.Round(this.buttonsItemsPresenter.ActualHeight / buttonHeight);
        }
    }
}
