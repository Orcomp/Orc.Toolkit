// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBar.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The status bar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.Windows.Controls;

    /// <summary>
    /// The status bar.
    /// </summary>
    public class StatusBar : ItemsControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        public StatusBar()
        {
            this.DefaultStyleKey = typeof(StatusBar);
        }

        #endregion
    }
}