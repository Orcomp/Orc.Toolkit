using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Orc.Toolkit
{
    public class AttributeConfigurationEditorProperty : DependencyObject
    {
        public AttributeConfigurationEditorProperty()
        {
        }

        public bool ShowInAllSections
        {
            get { return (bool)GetValue(ShowInAllSectionsProperty); }
            set { SetValue(ShowInAllSectionsProperty, value); }
        }

        public static readonly DependencyProperty ShowInAllSectionsProperty =
            DependencyProperty.Register("ShowInAllSections", typeof(bool), typeof(AttributeConfigurationEditorProperty), new PropertyMetadata(false));


        [Bindable(true)]
        public IEnumerable SupportedValues
        {
            get { return (IEnumerable)GetValue(SupportedValuesProperty); }
            set { SetValue(SupportedValuesProperty, value); }
        }
        
        public static readonly DependencyProperty SupportedValuesProperty =
            DependencyProperty.Register("SupportedValues", typeof(IEnumerable), 
            typeof(AttributeConfigurationEditorProperty), new PropertyMetadata(null));



        public string SupportedValuesDisplayMemberPath
        {
            get { return (string)GetValue(SupportedValuesDisplayMemberPathProperty); }
            set { SetValue(SupportedValuesDisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty SupportedValuesDisplayMemberPathProperty =
            DependencyProperty.Register("SupportedValuesDisplayMemberPath", typeof(string), typeof(AttributeConfigurationEditorProperty), new PropertyMetadata(""));



        public string SupportedValuesValuePath
        {
            get { return (string)GetValue(SupportedValuesValuePathProperty); }
            set { SetValue(SupportedValuesValuePathProperty, value); }
        }

        public static readonly DependencyProperty SupportedValuesValuePathProperty =
            DependencyProperty.Register("SupportedValuesValuePath", typeof(string), typeof(AttributeConfigurationEditorProperty), new PropertyMetadata(""));




        public string BindingPath
        {
            get { return (string)GetValue(BindingPathProperty); }
            set { SetValue(BindingPathProperty, value); }
        }

        public static readonly DependencyProperty BindingPathProperty =
            DependencyProperty.Register("BindingPath", typeof(string), typeof(AttributeConfigurationEditorProperty),
            new PropertyMetadata(""));

        
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(AttributeConfigurationEditorProperty), 
            new PropertyMetadata(null));


    }
}
