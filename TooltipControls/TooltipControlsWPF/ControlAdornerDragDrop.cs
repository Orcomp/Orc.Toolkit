// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlAdornerDragDrop.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls
{
    using System.Windows;
    using System.Windows.Input;

    using Orc.Toolkit.TooltipControls.Helpers;

    public class ControlAdornerDragDrop
    {
        private ControlAdorner adorner;
        private double mouseX, mouseY;
        private bool mouseCaptured;

        private ControlAdornerDragDrop()
        {
        }

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

            var deltaY = e.GetPosition(null).Y - this.mouseY;
            var deltaX = e.GetPosition(null).X - this.mouseX;

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

            var windowSize = ScreenUtils.GetWindowSize();

            var maxY = windowSize.Height - frameworkElement.ActualHeight;
            if (this.adorner.ChildPosition.Y + offset.Y > maxY)
            {
                offset.Y = maxY - this.adorner.ChildPosition.Y;
            }

            var maxX = windowSize.Width - frameworkElement.ActualWidth;
            if (this.adorner.ChildPosition.X + offset.X > maxX)
            {
                offset.X = maxX - this.adorner.ChildPosition.X; 
            }

            this.adorner.Offset = offset;

            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }

        private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.adorner.Child.ReleaseMouseCapture();
            this.mouseCaptured = false;
            this.mouseY = 0;
            this.mouseX = 0;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.adorner.Child.CaptureMouse();
            this.mouseCaptured = true;
            this.mouseY = e.GetPosition(null).Y;
            this.mouseX = e.GetPosition(null).X;
        }
    }
}
