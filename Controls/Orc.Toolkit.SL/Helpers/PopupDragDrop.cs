// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupDragDrop.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   A class that makes popups moveable.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Helpers
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// A class that makes popups moveable.
    /// </summary>
    internal class PopupDragDrop
    {
        #region Fields

        /// <summary>
        /// The mouse captured.
        /// </summary>
        private bool mouseCaptured;

        /// <summary>
        /// The mouse x.
        /// </summary>
        private double mouseX;

        /// <summary>
        /// The mouse y.
        /// </summary>
        private double mouseY;

        /// <summary>
        /// The popup.
        /// </summary>
        private Popup popup;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="PopupDragDrop"/> class from being created.
        /// </summary>
        private PopupDragDrop()
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="popup">
        /// The popup.
        /// </param>
        /// <returns>
        /// The <see cref="PopupDragDrop"/>.
        /// </returns>
        public static PopupDragDrop Attach(Popup popup)
        {
            if (popup == null || popup.Child == null || !(popup.Child is FrameworkElement))
            {
                return null;
            }

            var pdd = new PopupDragDrop { popup = popup };
            pdd.popup.Child.MouseLeftButtonDown += pdd.MouseLeftButtonDown;
            pdd.popup.Child.MouseLeftButtonUp += pdd.MouseLeftButtonUp;
            pdd.popup.Child.MouseMove += pdd.MouseMove;
            return pdd;
        }

        /// <summary>
        /// The detach.
        /// </summary>
        /// <param name="pdd">
        /// The pdd.
        /// </param>
        public static void Detach(PopupDragDrop pdd)
        {
            if (pdd == null || pdd.popup == null || pdd.popup.Child == null)
            {
                return;
            }

            pdd.popup.Child.MouseLeftButtonDown -= pdd.MouseLeftButtonDown;
            pdd.popup.Child.MouseLeftButtonUp -= pdd.MouseLeftButtonUp;
            pdd.popup.Child.MouseMove -= pdd.MouseMove;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The mouse left button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.popup.Child.CaptureMouse();
            this.mouseCaptured = true;
            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }

        /// <summary>
        /// The mouse left button up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.popup.Child.ReleaseMouseCapture();
            this.mouseCaptured = false;
            this.mouseY = 0;
            this.mouseX = 0;
        }

        /// <summary>
        /// The mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.mouseCaptured)
            {
                return;
            }

            var frameworkElement = this.popup.Child as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            double deltaY = e.GetPosition(null).Y - this.mouseY;
            double deltaX = e.GetPosition(null).X - this.mouseX;
            this.popup.VerticalOffset += deltaY;
            this.popup.HorizontalOffset += deltaX;

            // constrain the popup to the bounds of the silverlight window
            if (this.popup.VerticalOffset < 0)
            {
                this.popup.VerticalOffset = 0;
            }

            if (this.popup.HorizontalOffset < 0)
            {
                this.popup.HorizontalOffset = 0;
            }

            Size windowSize = ScreenUtils.GetWindowSize();

            double maxY = windowSize.Height - frameworkElement.ActualHeight;
            if (this.popup.VerticalOffset > maxY)
            {
                this.popup.VerticalOffset = maxY;
            }

            double maxX = windowSize.Width - frameworkElement.ActualWidth;
            if (this.popup.HorizontalOffset > maxX)
            {
                this.popup.HorizontalOffset = maxX;
            }

            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }

        #endregion
    }
}