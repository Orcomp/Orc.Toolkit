using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Orc.Toolkit
{
    [ContentPropertyAttribute("Properties")]
    public class AttributeConfigurationEditorSection : DependencyObject
    {
        public AttributeConfigurationEditorSection()
        {
            Properties = new ObservableCollection<AttributeConfigurationEditorProperty>();
        }

        [Bindable(true)]
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(AttributeConfigurationEditorSection), new PropertyMetadata(null));



        public Image HeaderImage
        {
            get { return (Image)GetValue(HeaderImageProperty); }
            set { SetValue(HeaderImageProperty, value); }
        }

        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register("HeaderImage", typeof(Image), typeof(AttributeConfigurationEditorSection), new PropertyMetadata(null));



        [Bindable(true)]
        public ObservableCollection<AttributeConfigurationEditorProperty> Properties
        {
            get { return (ObservableCollection<AttributeConfigurationEditorProperty>)GetValue(PropertiesProperty); }
            set { SetValue(PropertiesProperty, value); }
        }
        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(ObservableCollection<AttributeConfigurationEditorProperty>),
            typeof(AttributeConfigurationEditorSection), new PropertyMetadata(null));


    }
}
