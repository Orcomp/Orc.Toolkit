// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlAdorner.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   An adorner class that contains a control as only child.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    ///     An adorner class that contains a control as only child.
    /// </summary>
    internal class ControlAdorner : Adorner
    {
        #region Fields

        /// <summary>
        ///     The child.
        /// </summary>
        private Control child;

        /// <summary>
        ///     The offset.
        /// </summary>
        private Point offset;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        public ControlAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.ChildPosition = new Point();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the child.
        /// </summary>
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

        /// <summary>
        ///     Gets the child position.
        /// </summary>
        public Point ChildPosition { get; private set; }

        /// <summary>
        ///     Gets or sets the horizontal offset.
        /// </summary>
        public int HorizontalOffset { get; set; }

        /// <summary>
        ///     Gets or sets the offset.
        /// </summary>
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

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the visual children count.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="finalSize">
        /// The final size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
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
            else if (c != null)
            {
                this.ChildPosition = c.GetPosition();
                rect = new Rect(this.ChildPosition.X, this.ChildPosition.Y, finalSize.Width, finalSize.Height);
            }
            else
            {
                rect = new Rect(this.offset.X, this.offset.Y, finalSize.Width, finalSize.Height);
            }

            this.child.Arrange(rect);
            return new Size(this.child.ActualWidth, this.child.ActualHeight);
        }

        /// <summary>
        /// The get visual child.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Visual"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.child;
        }

        /// <summary>
        /// The measure override.
        /// </summary>
        /// <param name="constraint">
        /// The constraint.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        #endregion
    }
}