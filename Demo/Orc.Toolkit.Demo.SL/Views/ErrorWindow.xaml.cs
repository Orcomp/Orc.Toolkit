// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorWindow.xaml.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The error window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ToolkitDemoSL
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The error window.
    /// </summary>
    public partial class ErrorWindow : ChildWindow
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public ErrorWindow(Exception e)
        {
            this.InitializeComponent();
            if (e != null)
            {
                this.ErrorTextBox.Text = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        public ErrorWindow(Uri uri)
        {
            this.InitializeComponent();
            if (uri != null)
            {
                this.ErrorTextBox.Text = "Page not found: \"" + uri + "\"";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        public ErrorWindow(string message, string details)
        {
            this.InitializeComponent();
            this.ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The ok button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        #endregion
    }
}