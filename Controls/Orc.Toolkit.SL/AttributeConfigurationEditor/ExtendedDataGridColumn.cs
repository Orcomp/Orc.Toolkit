using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ExtendedDataGridColumn : DataGridColumn
    {
        public ExtendedDataGridColumn()
        {
        }

        public DataGrid DataGrid
        {
            get { return (DataGrid)GetValue(DataGridProperty); }
            set { SetValue(DataGridProperty, value); }
        }public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register("DataGrid", typeof(DataGrid), typeof(ExtendedDataGridColumn), new PropertyMetadata(null));
        
        public string BindingPath
        {
            get { return (string)GetValue(BindingPathProperty); }
            set { SetValue(BindingPathProperty, value); }
        }
        public static readonly DependencyProperty BindingPathProperty =
            DependencyProperty.Register("BindingPath", typeof(string), typeof(ExtendedDataGridColumn), new PropertyMetadata(""));



        public IEnumerable SupportedValues
        {
            get { return (IEnumerable)GetValue(SupportedValuesProperty); }
            set { SetValue(SupportedValuesProperty, value); }
        }

        public static readonly DependencyProperty SupportedValuesProperty =
            DependencyProperty.Register("SupportedValues", typeof(IEnumerable), typeof(ExtendedDataGridColumn), 
            new PropertyMetadata(null));

        public string SupportedValuesDisplayMemberPath
        {
            get { return (string)GetValue(SupportedValuesDisplayMemberPathProperty); }
            set { SetValue(SupportedValuesDisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty SupportedValuesDisplayMemberPathProperty =
            DependencyProperty.Register("SupportedValuesDisplayMemberPath", typeof(string), typeof(ExtendedDataGridColumn), new PropertyMetadata(""));



        public string SupportedValuesValuePath
        {
            get { return (string)GetValue(SupportedValuesValuePathProperty); }
            set { SetValue(SupportedValuesValuePathProperty, value); }
        }

        public static readonly DependencyProperty SupportedValuesValuePathProperty =
            DependencyProperty.Register("SupportedValuesValuePath", typeof(string), typeof(ExtendedDataGridColumn), new PropertyMetadata(""));


        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var contentPresenter = new ContentPresenter();
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if ((dataItem.GetType().GetProperty(this.BindingPath) != null))
                {
                    if (this.SupportedValues != null)
                    {
                        var comboBox = new ComboBox();
                        comboBox.IsHitTestVisible = false;
                        var itemsSourceBinding = new Binding("SupportedValues") { Source = this };
                        comboBox.SetBinding(ComboBox.ItemsSourceProperty, itemsSourceBinding);
                        comboBox.DisplayMemberPath = this.SupportedValuesDisplayMemberPath;
                        comboBox.SelectedValuePath = this.SupportedValuesValuePath;
                        comboBox.IsSynchronizedWithCurrentItem = false;
                        comboBox.SetBinding(ComboBox.SelectedValueProperty, new Binding(this.BindingPath) { Source = dataItem, Mode = BindingMode.TwoWay });
                        contentPresenter.Content = comboBox;
                    }
                    else
                    {
                        string typeName = dataItem.GetType().GetProperty(this.BindingPath).PropertyType.FullName;
                        switch (typeName)
                        {
                            case "System.Int32":
                            case "System.Int64":
                            case "System.String":                            
                                {
                                    var textBlock = new TextBlock();
                                    textBlock.SetBinding(TextBlock.TextProperty, new Binding(this.BindingPath) { Source = dataItem });
                                    contentPresenter.Content = textBlock;
                                    break;
                                }
                            case "System.Boolean":
                                {
                                    var checkBox = new CheckBox();
                                    checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding(this.BindingPath) { Source = dataItem });
                                    checkBox.IsEnabled = false;
                                    contentPresenter.Content = checkBox;
                                    break;
                                }
                        }
                    }
                }
            }));
            return contentPresenter;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var contentPresenter = new ContentPresenter();
            //Dispatcher.BeginInvoke((Action)(() =>
            //{
                if ((dataItem.GetType().GetProperty(this.BindingPath) != null))
                {
                    if (this.SupportedValues != null)
                    {
                        var comboBox = new ComboBox();
                        var itemsSourceBinding = new Binding("SupportedValues") { Source = this };
                        comboBox.SetBinding(ComboBox.ItemsSourceProperty, itemsSourceBinding);                        
                        comboBox.DisplayMemberPath = this.SupportedValuesDisplayMemberPath;
                        comboBox.SelectedValuePath = this.SupportedValuesValuePath; 
                        comboBox.IsSynchronizedWithCurrentItem = false;  
                        comboBox.SetBinding(ComboBox.SelectedValueProperty, new Binding(this.BindingPath) { Source = dataItem, Mode = BindingMode.TwoWay });
                        contentPresenter.Content = comboBox;
                    }
                    else
                    {
                        string typeName = dataItem.GetType().GetProperty(this.BindingPath).PropertyType.FullName;
                        switch (typeName)
                        {
                            case "System.String":
                            case "System.Int32":
                            case "System.Int64":
                                {
                                    var textBox = new TextBox();
                                    textBox.SetBinding(TextBox.TextProperty, new Binding(this.BindingPath) { Source = dataItem, Mode = BindingMode.TwoWay });
                                    contentPresenter.Content = textBox;
                                    break;
                                }
                            case "System.Boolean":
                                {
                                    var checkBox = new CheckBox();
                                    checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding(this.BindingPath) { Source = dataItem, Mode = BindingMode.TwoWay });
                                    contentPresenter.Content = checkBox;
                                    break;
                                }
                        }
                    }
                }
            //}));
            return contentPresenter;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            if(editingElement is ContentPresenter)
            {
                var element = (editingElement as ContentPresenter).Content;
                if (element is TextBox)
                {
                    var textBox = (TextBox)element;
                    textBox.Focus();
                    textBox.SelectAll();
                    if (textBox != null)
                    {
                        return textBox.Text;
                    }
                }
                if (element is CheckBox)
                {
                    var checkBox = (CheckBox)element;
                    checkBox.Focus();
                    if (checkBox != null)
                    {
                        return checkBox.GetValue(CheckBox.IsCheckedProperty);
                    }
                }
                if (element is ComboBox)
                {
                    var comboBox = (ComboBox)element;
                    comboBox.Focus();
                    if (comboBox != null)
                    {
                        return comboBox.SelectedValue;
                    }
                }
            }
            

            return "Not implemented!";
        }

        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            base.RefreshCellContent(element, propertyName);
        }

        protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
        {
            if (editingElement is ContentPresenter)
            {
                var element = (editingElement as ContentPresenter).Content;
                
                if (element is TextBox)
                {
                    var textBox = (TextBox)element;
                    textBox.Text = (string)uneditedValue;
                }
                if (element is CheckBox)
                {
                    var checkBox = (CheckBox)element;
                    checkBox.SetValue(CheckBox.IsCheckedProperty, (bool)uneditedValue);
                }
                if (element is ComboBox)
                {
                    var comboBox = (ComboBox)element;
                    comboBox.SetValue(ComboBox.SelectedValueProperty, uneditedValue);
                }
            }

            
        }
    }
}
