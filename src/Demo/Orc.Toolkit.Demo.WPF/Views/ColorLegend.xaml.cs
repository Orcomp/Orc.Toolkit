// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorLegend.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Interaction logic for ColorLegend.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Demo.Views
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for ColorLegend.xaml
    /// </summary>
    public partial class ColorLegend
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorLegend"/> class.
        /// </summary>
        public ColorLegend()
        {
            InitializeComponent();
            this.FillLegend(extendedColorLegend1);
        }

        /// <summary>
        /// The fill legend.
        /// </summary>
        /// <param name="elc">
        /// The elc.
        /// </param>
        public void FillLegend(ExtendedColorLegend elc)
        {
            var colors = new ObservableCollection<IColorProvider>
                             {
                                 new DemoColorProvider
                                     {
                                         Color = Colors.Red,
                                         IsVisible = true,
                                         Description = "Red",
                                         AdditionalData = "(5)"
                                     },
                                 new DemoColorProvider
                                     {
                                         Color = Colors.Yellow,
                                         IsVisible = true,
                                         Description = "Yellow",
                                         AdditionalData = "(4)"
                                     },
                                 new DemoColorProvider
                                     {
                                         Color = Colors.Green,
                                         IsVisible = true,
                                         Description = "Green",
                                         AdditionalData = "(3)"
                                     }
                             };

            elc.ItemsSource = colors;
        }

        /// <summary>
        /// The check box 1_ checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBox1Checked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSearchBox = true;
        }

        /// <summary>
        /// The cb show search box_ on unchecked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowSearchBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSearchBox = false;
        }

        /// <summary>
        /// The cb show settings_ checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowSettingsChecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSettings = true;
        }

        /// <summary>
        /// The cb show settings_ unchecked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowSettingsUnchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowSettings = false;
        }

        /// <summary>
        /// The cb show toolbox_ checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowToolboxChecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowToolBox = true;
        }

        /// <summary>
        /// The cb show toolbox_ unchecked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowToolboxUnchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowToolBox = false;
        }

        /// <summary>
        /// The cb show color visibility_ checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowColorVisibilityChecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowColorVisibilityControls = true;
        }

        /// <summary>
        /// The cb show color visibility_ unchecked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbShowColorVisibilityUnchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.ShowColorVisibilityControls = false;
        }

        /// <summary>
        /// The cb allow color editing_ checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbAllowColorEditingChecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.AllowColorEditing = true;
        }

        /// <summary>
        /// The cb allow color editing_ unchecked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CbAllowColorEditingUnchecked(object sender, RoutedEventArgs e)
        {
            extendedColorLegend1.AllowColorEditing = false;
        }
    }

    /// <summary>
    /// The demo color provider.
    /// </summary>
    public class DemoColorProvider : IColorProvider
    {
        /// <summary>
        /// The is visible.
        /// </summary>
        private bool isVisible;

        /// <summary>
        /// The color.
        /// </summary>
        private Color color;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// The property changed.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IColorProvider Members

        /// <summary>
        /// Gets or sets a value indicating whether is visible.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        public string AdditionalData { get; set; }

        #endregion
    }
}
