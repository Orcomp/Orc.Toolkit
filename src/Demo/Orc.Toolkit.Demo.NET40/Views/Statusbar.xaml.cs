// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Statusbar.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Interaction logic for Statusbar.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Demo.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    ///     Interaction logic for Statusbar.xaml
    /// </summary>
    public partial class Statusbar : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Statusbar"/> class.
        /// </summary>
        public Statusbar()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cnv_ mouse move_1.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void cnv_MouseMove_1(object sender, MouseEventArgs e)
        {
            this.pointPosition.Text = e.GetPosition(this.cnv).ToString();
        }

        #endregion
    }
}