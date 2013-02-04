// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tooltips.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The tooltips.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ToolkitDemoSL
{
    using System.Windows.Controls;
    using System.Windows.Navigation;

    /// <summary>
    /// The tooltips.
    /// </summary>
    public partial class Tooltips : Page
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tooltips"/> class.
        /// </summary>
        public Tooltips()
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

        #endregion
    }
}