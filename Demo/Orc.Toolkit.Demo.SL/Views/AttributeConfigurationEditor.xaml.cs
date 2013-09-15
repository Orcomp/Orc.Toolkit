using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using Orc.Toolkit.Demo.Models;
using System.Collections;

namespace Orc.Toolkit.Demo.Views
{
    public partial class AttributeConfigurationEditor : Page
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

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

    }
}
