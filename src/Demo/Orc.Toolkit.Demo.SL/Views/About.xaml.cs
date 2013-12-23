// --------------------------------------------------------------------------------------------------------------------
// <copyright file="About.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The about.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ToolkitDemoSL
{
    using System.Windows.Controls;
    using System.Windows.Navigation;

    /// <summary>
    /// The about.
    /// </summary>
    public partial class About : Page
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About()
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