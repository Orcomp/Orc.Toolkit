// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Printing.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The printing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Demo.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;

    /// <summary>
    /// The printing.
    /// </summary>
    public partial class Printing : Page
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Printing"/> class.
        /// </summary>
        public Printing()
        {
            this.InitializeComponent();
        }

        #endregion

        // Executes when the user navigates to this page.
        #region Methods

        /// <summary>
        /// The on navigated to.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// The btn print_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource bs = CanvasToPrint.PrintToPrinter(this.canvas1, this.canvas2, this.canvas3, this.canvas4);
            this.OutputImage.Source = bs;
        }

        /// <summary>
        /// The btn save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "png files (*.png)|*.png|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == true)
            {
                BitmapSource bs = CanvasToPrint.PrintToPngFile(
                    this.canvas1, this.canvas2, this.canvas3, this.canvas4, saveFileDialog1.OpenFile());
                this.OutputImage.Source = bs;
            }
        }

        #endregion
    }
}