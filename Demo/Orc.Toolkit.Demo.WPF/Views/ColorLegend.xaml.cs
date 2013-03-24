using System;
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for ColorLegend.xaml
    /// </summary>
    public partial class ColorLegend : UserControl
    {
        public ColorLegend()
        {
            InitializeComponent();
            FillLegend(extendedColorLegend1);
        }

        public void FillLegend(ExtendedColorLegend elc)
        {
            var colors = new ObservableCollection<IColorProvider>();
            colors.Add(new DemoColorProvider() { Color = Colors.Red, IsVisible = true, Description = "Red", AdditionalData = "(5)"});
            colors.Add(new DemoColorProvider() { Color = Colors.Yellow, IsVisible = true, Description = "Yellow", AdditionalData = "(4)" });
            colors.Add(new DemoColorProvider() { Color = Colors.Green, IsVisible = true, Description = "Green", AdditionalData = "(3)" });
            elc.ItemsSource = colors;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSearchBox = true;
        }

        private void CbShowSearchBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSearchBox = false;
        }

        private void cbShowSettings_Checked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSettings = true;
        }

        private void cbShowSettings_Unchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSettings = false;
        }

        private void cbShowToolbox_Checked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowToolBox = true;
        }

        private void cbShowToolbox_Unchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowToolBox = false;
        }

        private void cbShowColorVisibility_Checked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowColorVisibilityControls = true;
        }

        private void cbShowColorVisibility_Unchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowColorVisibilityControls = false;
        }

        private void cbAllowColorEditing_Checked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.AllowColorEditing = true;
        }

        private void cbAllowColorEditing_Unchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.AllowColorEditing = false;
        }
    }

    public class DemoColorProvider : IColorProvider
    {
        private bool isVisible;

        private Color color;

        private string description;

        #region IColorProvider Members

        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
            }
        }

        public System.Windows.Media.Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
                
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public string Id { get; set; }

        public string AdditionalData { get; set; }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
