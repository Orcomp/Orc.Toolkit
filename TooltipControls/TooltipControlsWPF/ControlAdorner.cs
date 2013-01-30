// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlAdorner.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class ControlAdorner : Adorner
    {
        private Control child;

        private Point offset;

        public ControlAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.ChildPosition = new Point();
        }

        public Control Child
        {
            get
            {
                return this.child;
            }

            set
            {
                if (this.child != null)
                {
                    this.RemoveVisualChild(this.child);
                }

                this.child = value;
                if (this.child != null)
                {
                    this.AddVisualChild(this.child);
                }
            }
        }

        public Point ChildPosition { get; private set; }

        public Point Offset
        {
            get
            {
                return this.offset;
            }

            set
            {
                this.offset = value;
                this.InvalidateArrange();
            }
        }

        public int HorizontalOffset { get; set; }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.child;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var c = this.child as IControlAdornerChild;
            Rect rect;

            if (this.Offset.X != 0 || this.Offset.Y != 0)
            {
                rect = new Rect(
                    this.ChildPosition.X + this.Offset.X,
                    this.ChildPosition.Y + this.Offset.Y,
                    finalSize.Width,
                    finalSize.Height); 
            }
            else
            if (c != null)
            {
                this.ChildPosition = c.GetPosition();
                rect = new Rect(
                    this.ChildPosition.X,
                    this.ChildPosition.Y,
                    finalSize.Width, 
                    finalSize.Height);    
            }
            else
            {
                rect = new Rect(this.offset.X, this.offset.Y, finalSize.Width, finalSize.Height);
            }

            this.child.Arrange(rect);
            return new Size(this.child.ActualWidth, this.child.ActualHeight);
        }
    }
}
