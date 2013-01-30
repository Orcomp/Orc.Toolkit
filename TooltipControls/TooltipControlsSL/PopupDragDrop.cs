// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupDragDrop.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls
{
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Orc.Toolkit.TooltipControls.Helpers;

    public class PopupDragDrop
    {
        private Popup popup;
        private double mouseX, mouseY;
        private bool mouseCaptured;

        private PopupDragDrop()
        {
        }

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

            var deltaY = e.GetPosition(null).Y - this.mouseY;
            var deltaX = e.GetPosition(null).X - this.mouseX;
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

            var windowSize = ScreenUtils.GetWindowSize();

            var maxY = windowSize.Height - frameworkElement.ActualHeight;
            if (this.popup.VerticalOffset > maxY)
            {
                this.popup.VerticalOffset = maxY;
            }

            var maxX = windowSize.Width - frameworkElement.ActualWidth;
            if (this.popup.HorizontalOffset > maxX)
            {
                this.popup.HorizontalOffset = maxX;
            }

            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.popup.Child.ReleaseMouseCapture();
            this.mouseCaptured = false;
            this.mouseY = 0;
            this.mouseX = 0;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.popup.Child.CaptureMouse();
            this.mouseCaptured = true;
            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }
    }
}
