using Orc.Toolkit.Demo.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Orc.Toolkit.Demo.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationEditor.xaml
    /// </summary>
    public partial class AttributeConfigurationEditor : UserControl
    {
        public AttributeConfigurationEditor()
        {
            SupportedTypes = AttributeConfiguration.SupportedTypes;

            InitializeComponent();
        }



        public IEnumerable SupportedTypes
        {
            get { return (IEnumerable)GetValue(SupportedTypesProperty); }
            set { SetValue(SupportedTypesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SupportedTypes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SupportedTypesProperty =
            DependencyProperty.Register("SupportedTypes", typeof(IEnumerable), typeof(AttributeConfigurationEditor), 
            new PropertyMetadata(null));


    }
}
