// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlAdornerDragDrop.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   A class that makes ControlAdorner moveable.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Orc.Toolkit.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    ///     A class that makes ControlAdorner moveable.
    /// </summary>
    internal class ControlAdornerDragDrop
    {
        #region Fields

        /// <summary>
        ///     The adorner.
        /// </summary>
        private ControlAdorner adorner;

        /// <summary>
        ///     The mouse captured.
        /// </summary>
        private bool mouseCaptured;

        /// <summary>
        ///     The mouse x.
        /// </summary>
        private double mouseX;

        /// <summary>
        ///     The mouse y.
        /// </summary>
        private double mouseY;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="ControlAdornerDragDrop" /> class from being created.
        /// </summary>
        private ControlAdornerDragDrop()
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="adorner">
        /// The adorner.
        /// </param>
        /// <returns>
        /// The <see cref="ControlAdornerDragDrop"/>.
        /// </returns>
        public static ControlAdornerDragDrop Attach(ControlAdorner adorner)
        {
            if (adorner == null || adorner.Child == null)
            {
                return null;
            }

            var dd = new ControlAdornerDragDrop { adorner = adorner };
            dd.adorner.Child.MouseLeftButtonDown += dd.MouseLeftButtonDown;
            dd.adorner.Child.MouseLeftButtonUp += dd.MouseLeftButtonUp;
            dd.adorner.Child.MouseMove += dd.MouseMove;
            return dd;
        }

        /// <summary>
        /// The detach.
        /// </summary>
        /// <param name="dd">
        /// The dd.
        /// </param>
        public static void Detach(ControlAdornerDragDrop dd)
        {
            if (dd == null || dd.adorner == null || dd.adorner.Child == null)
            {
                return;
            }

            dd.adorner.Child.MouseLeftButtonDown -= dd.MouseLeftButtonDown;
            dd.adorner.Child.MouseLeftButtonUp -= dd.MouseLeftButtonUp;
            dd.adorner.Child.MouseMove -= dd.MouseMove;
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
            this.adorner.Child.CaptureMouse();
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
            this.adorner.Child.ReleaseMouseCapture();
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

            var frameworkElement = this.adorner as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }

            double deltaY = e.GetPosition(null).Y - this.mouseY;
            double deltaX = e.GetPosition(null).X - this.mouseX;

            var offset = new Point(this.adorner.Offset.X + deltaX, this.adorner.Offset.Y + deltaY);

            // constrain the popup to the bounds of the window
            if (this.adorner.ChildPosition.Y + offset.Y < 0)
            {
                offset.Y = -this.adorner.ChildPosition.Y;
            }

            if (this.adorner.ChildPosition.X + offset.X < 0)
            {
                offset.X = -this.adorner.ChildPosition.X;
            }

            Size boundariesSize = ScreenUtils.GetWindowSize();
            var adornedElement = this.adorner.AdornedElement as FrameworkElement;
            if (adornedElement != null)
            {
                boundariesSize = new Size(adornedElement.ActualWidth, adornedElement.ActualHeight);
            }

            double maxY = boundariesSize.Height - frameworkElement.ActualHeight;
            if (this.adorner.ChildPosition.Y + offset.Y > maxY)
            {
                offset.Y = maxY - this.adorner.ChildPosition.Y;
            }

            double maxX = boundariesSize.Width - frameworkElement.ActualWidth;
            if (this.adorner.ChildPosition.X + offset.X > maxX)
            {
                offset.X = maxX - this.adorner.ChildPosition.X;
            }

            this.adorner.Offset = offset;

            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }

        #endregion
    }
}

#endif