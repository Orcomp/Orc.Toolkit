// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Statusbar.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The statusbar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Demo.Views
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;

    /// <summary>
    /// The statusbar.
    /// </summary>
    public partial class Statusbar : Page
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