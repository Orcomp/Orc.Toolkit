// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PinnableTooltip.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The pinnable tooltip control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using Orc.Toolkit.Commands;
    using Orc.Toolkit.Helpers;

    /// <summary>
    ///     The pinnable tooltip control.
    /// </summary>
    [TemplatePart(Name = "PinButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "DragGrip", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "GripDrawing", Type = typeof(GeometryDrawing))]
    public class PinnableTooltip : ContentControl, IControlAdornerChild
    {
        #region Constants

        /// <summary>
        ///     The epsilon.
        /// </summary>
        private const double Epsilon = 1E-7;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The horizontal offset property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset", 
                typeof(double), 
                typeof(PinnableTooltip), 
                new PropertyMetadata(OnHorizontalOffsetPropertyChanged));
        
        /// <summary>
        ///     The is pinned property.
        /// </summary>
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
            "IsPinned", typeof(bool), typeof(PinnableTooltip), new PropertyMetadata(false, OnIsPinnedPropertyChanged));

        /// <summary>
        ///     The vertical offset property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
            "VerticalOffset", 
            typeof(double), 
            typeof(PinnableTooltip), 
            new PropertyMetadata(OnVerticalOffsetPropertyChanged));

        /// <summary>
        /// The grip brush property.
        /// </summary>
        public static readonly DependencyProperty GripColorProperty = DependencyProperty.Register(
            "GripColor",
            typeof(Color),
            typeof(PinnableTooltip),
            new PropertyMetadata(Color.FromRgb(204, 204, 204), OnGripColorChanged));

        //Color.FromRgb(204, 204, 204)
        #endregion

        #region Fields

        /// <summary>
        ///     The drag grip.
        /// </summary>
        private FrameworkElement dragGrip;

        /// <summary>
        /// The grip drawing.
        /// </summary>
        private GeometryDrawing gripDrawing;

        /// <summary>
        ///     The adorner.
        /// </summary>
        private ControlAdorner adorner;

        /// <summary>
        ///     The adorner drag drop.
        /// </summary>
        private ControlAdornerDragDrop adornerDragDrop;

        /// <summary>
        ///     The adorner layer.
        /// </summary>
        private AdornerLayer adornerLayer;

        /// <summary>
        ///     The is position calculated.
        /// </summary>
        private bool isPositionCalculated;

        /// <summary>
        ///     The last position.
        /// </summary>
        private Point lastPosition;

        /// <summary>
        ///     The last size.
        /// </summary>
        private Size lastSize;

        /// <summary>
        ///     The owner.
        /// </summary>
        private UIElement owner;

        /// <summary>
        /// The adorner.
        /// </summary>
        private UIElement userDefinedAdorner;

        /// <summary>
        ///     The timer.
        /// </summary>
        private TooltipTimer timer;

        /// <summary>
        /// Change color command
        /// </summary>
        private ICommand openLinkCommand;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PinnableTooltip" /> class.
        /// </summary>
        public PinnableTooltip()
        {
            this.DefaultStyleKey = typeof(PinnableTooltip);
            this.SizeChanged += this.OnSizeChanged;
            this.MouseEnter += this.OnPinnableTooltipMouseEnter;
            this.MouseLeave += this.OnPinnableTooltipMouseLeave;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the horizontal offset.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return (double)this.GetValue(HorizontalOffsetProperty);
            }

            set
            {
                this.SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.adorner != null;
            }

            set
            {
                if (value)
                {
                    this.CreateAdorner();
                    return;
                }

                this.RemoveAdorner();

                if (this.IsTimerEnabled)
                {
                    this.timer.StopAndReset();
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is pinned.
        /// </summary>
        public bool IsPinned
        {
            get
            {
                return (bool)this.GetValue(IsPinnedProperty);
            }

            set
            {
                this.SetValue(IsPinnedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the grip brush.
        /// </summary>
        public Color GripColor
        {
            get
            {
                return (Color)this.GetValue(GripColorProperty);
            }

            set
            {
                this.SetValue(GripColorProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the vertical offset.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return (double)this.GetValue(VerticalOffsetProperty);
            }

            set
            {
                this.SetValue(VerticalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets the color change command
        /// </summary>
        public ICommand OpenLinkCommand
        {
            get
            {
                return this.openLinkCommand ?? (this.openLinkCommand = new DelegateCommand(
                    o =>
                    {
                        MessageBox.Show("I am open link command");
                    },
                    p => true));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether is timer enabled.
        /// </summary>
        internal bool IsTimerEnabled
        {
            get
            {
                return this.timer != null && this.timer.IsEnabled;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The get position.
        /// </summary>
        /// <returns>
        ///     The <see cref="Point" />.
        /// </returns>
        public Point GetPosition()
        {
            var position = new Point();

            Point mousePosition = PinnableTooltipService.MousePosition;
            FrameworkElement rootVisual = PinnableTooltipService.RootVisual;

            double horizontalOffset = this.HorizontalOffset;
            double verticalOffset = this.VerticalOffset;

            //using this code for non UIElements
            if (this.owner == null)
            {
                mousePosition = Mouse.GetPosition(this.userDefinedAdorner);
#if (SILVERLIGHT)
                RootVisual = Application.Current.RootVisual as FrameworkElement;
#else
                if ((this.userDefinedAdorner as FrameworkElement) == null)
                {
                    rootVisual = System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted
                                     ? null
                                     : (Application.Current.MainWindow.Content as FrameworkElement) != null
                                           ? Application.Current.MainWindow.Content as FrameworkElement
                                           : Application.Current.MainWindow;
                }
                else
                {
                    rootVisual = this.userDefinedAdorner as FrameworkElement;
                }

                if (rootVisual == null)
                {
                    if (this.isPositionCalculated)
                        position = this.lastPosition;
                    else
                        position = mousePosition;
                    return position;
                }
#endif

                if (this.isPositionCalculated)
                {
                    if (lastPosition.Y + this.DesiredSize.Height > rootVisual.ActualHeight)
                        this.lastPosition.Y = rootVisual.ActualHeight - this.DesiredSize.Height;
                    if (lastPosition.X + this.DesiredSize.Width > rootVisual.ActualWidth)
                        this.lastPosition.X = rootVisual.ActualWidth - this.DesiredSize.Width;
                    position = this.lastPosition;
                    return position;
                }

                double offsetX = mousePosition.X + horizontalOffset;
                double offsetY = mousePosition.Y + verticalOffset;

                //offsetX = Math.Max(2.0, offsetX);
                //offsetY = Math.Max(2.0, offsetY);

                double actualHeight = rootVisual.ActualHeight;
                double actualWidth = rootVisual.ActualWidth;
                double lastHeight = this.lastSize.Height;
                double lastWidth = this.lastSize.Width;

                var lastRectangle = new Rect(offsetX, offsetY, lastWidth, lastHeight);
                var actualRectangle = new Rect(0.0, 0.0, actualWidth, actualHeight);
                actualRectangle.Intersect(lastRectangle);

                if ((offsetY + this.DesiredSize.Height) > actualHeight)
                {
                    offsetY = (actualHeight - this.DesiredSize.Height) - 2.0;
                }

                if (offsetY < 0.0)
                {
                    offsetY = 0.0;
                }

                if ((offsetX + this.DesiredSize.Width) > actualWidth)
                {
                    offsetX = (actualWidth - this.DesiredSize.Width) - 2.0;
                }

                if (offsetX < 0.0)
                {
                    offsetX = 0.0;
                }

                position.Y = offsetY;
                position.X = offsetX;

                this.isPositionCalculated = this.DesiredSize.Height > 0;
                if (this.isPositionCalculated)
                {
                    this.lastPosition = position;
                }
                
                return position;
            }

            PlacementMode placementMode = PinnableTooltipService.GetPlacement(this.owner);
            UIElement placementTarget = PinnableTooltipService.GetPlacementTarget(this.owner) ?? this.owner;

            switch (placementMode)
            {
                case PlacementMode.Mouse:
                    if (this.isPositionCalculated)
                    {
                        position = this.lastPosition;
                        return position;
                    }

                    double offsetX = mousePosition.X + horizontalOffset;
                    double offsetY = mousePosition.Y + new TextBlock().FontSize + verticalOffset;

                    offsetX = Math.Max(2.0, offsetX);
                    offsetY = Math.Max(2.0, offsetY);

                    double actualHeight = rootVisual.ActualHeight;
                    double actualWidth = rootVisual.ActualWidth;
                    double lastHeight = this.lastSize.Height;
                    double lastWidth = this.lastSize.Width;

                    var lastRectangle = new Rect(offsetX, offsetY, lastWidth, lastHeight);
                    var actualRectangle = new Rect(0.0, 0.0, actualWidth, actualHeight);
                    actualRectangle.Intersect(lastRectangle);

                    if ((Math.Abs(actualRectangle.Width - lastRectangle.Width) < 2.0)
                        && (Math.Abs(actualRectangle.Height - lastRectangle.Height) < 2.0))
                    {
                        position.Y = offsetY;
                        position.X = offsetX;
                    }
                    else
                    {
                        if ((offsetY + lastRectangle.Height) > actualHeight)
                        {
                            offsetY = (actualHeight - lastRectangle.Height) - 2.0;
                        }

                        if (offsetY < 0.0)
                        {
                            offsetY = 0.0;
                        }

                        if ((offsetX + lastRectangle.Width) > actualWidth)
                        {
                            offsetX = (actualWidth - lastRectangle.Width) - 2.0;
                        }

                        if (offsetX < 0.0)
                        {
                            offsetX = 0.0;
                        }

                        position.Y = offsetY;
                        position.X = offsetX;
                    }

                    this.lastPosition = position;
                    this.isPositionCalculated = true;
                    break;
                case PlacementMode.Bottom:
                case PlacementMode.Right:
                case PlacementMode.Left:
                case PlacementMode.Top:
                    Size windowSize = ScreenUtils.GetWindowSize();
                    var plugin = new Rect(0.0, 0.0, windowSize.Width, windowSize.Height);
                    Point[] translatedPoints = GetTranslatedPoints((FrameworkElement)placementTarget);
                    Point[] toolTipPoints = GetTranslatedPoints(this);
                    Point popupLocation = PlacePopup(plugin, translatedPoints, toolTipPoints, placementMode);

                    position.Y = popupLocation.Y + verticalOffset;
                    position.X = popupLocation.X + horizontalOffset;
                    break;
            }

            return position;
        }

        /// <summary>
        ///     The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.dragGrip = (FrameworkElement)this.GetTemplateChild("DragGrip");
            if (this.dragGrip != null)
            {
                this.dragGrip.PreviewMouseLeftButtonDown += this.OnDragGripPreviewMouseLeftButtonDown;
            }

            this.gripDrawing = (GeometryDrawing)this.GetTemplateChild("GripDrawing");
        }
            
        #endregion

        #region Methods

        /// <summary>
        /// The calculate point.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="placement">
        /// The placement.
        /// </param>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="pointArray">
        /// The point array.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        private static Point CalculatePoint(
            IList<Point> target,
            PlacementMode placement,
            Rect plugin,
            double width,
            double height,
            IList<Point> pointArray,
            int index,
            Rect bounds)
        {
            double x = pointArray[index].X;
            double y = pointArray[index].Y;
            if (index > 1)
            {
                if ((placement == PlacementMode.Left) || (placement == PlacementMode.Right))
                {
                    if (((Math.Abs(y - target[0].Y) > Epsilon) && (Math.Abs(y - target[1].Y) > Epsilon))
                        && ((Math.Abs((y + height) - target[0].Y) > Epsilon)
                            && (Math.Abs((y + height) - target[1].Y) > Epsilon)))
                    {
                        double num18 = bounds.Top + (bounds.Height / 2.0);
                        if ((num18 > 0.0) && ((num18 - 0.0) > (plugin.Height - num18)))
                        {
                            y = plugin.Height - height;
                        }
                        else
                        {
                            y = 0.0;
                        }
                    }
                }
                else if (((placement == PlacementMode.Top) || (placement == PlacementMode.Bottom))
                         && (((Math.Abs(x - target[0].X) > 0.0001) && (Math.Abs(x - target[1].X) > 0.0001))
                             && ((Math.Abs((x + width) - target[0].X) > 0.0001) && (Math.Abs((x + width) - target[1].X) > 0.0001))))
                {
                    double num19 = bounds.Left + (bounds.Width / 2.0);
                    if ((num19 > 0.0) && ((num19 - 0.0) > (plugin.Width - num19)))
                    {
                        x = plugin.Width - width;
                    }
                    else
                    {
                        x = 0.0;
                    }
                }
            }

            return new Point(x, y);
        }

        /// <summary>
        /// The get bounds.
        /// </summary>
        /// <param name="interestPoints">
        /// The interest points.
        /// </param>
        /// <returns>
        /// The <see cref="Rect"/>.
        /// </returns>
        private static Rect GetBounds(params Point[] interestPoints)
        {
            double num2;
            double num4;
            double x = num2 = interestPoints[0].X;
            double y = num4 = interestPoints[0].Y;
            for (int i = 1; i < interestPoints.Length; i++)
            {
                double num6 = interestPoints[i].X;
                double num7 = interestPoints[i].Y;
                if (num6 < x)
                {
                    x = num6;
                }

                if (num6 > num2)
                {
                    num2 = num6;
                }

                if (num7 < y)
                {
                    y = num7;
                }

                if (num7 > num4)
                {
                    num4 = num7;
                }
            }

            return new Rect(x, y, (num2 - x) + 1.0, (num4 - y) + 1.0);
        }

        /// <summary>
        /// The get index.
        /// </summary>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <param name="pointArray">
        /// The point array.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetIndex(Rect plugin, double width, double height, IList<Point> pointArray)
        {
            double num13 = width * height;
            int index = 0;
            double num15 = 0.0;
            for (int i = 0; i < pointArray.Count; i++)
            {
                var rect3 = new Rect(pointArray[i].X, pointArray[i].Y, width, height);
                rect3.Intersect(plugin);
                double d = rect3.Width * rect3.Height;
                if (double.IsInfinity(d))
                {
                    index = pointArray.Count - 1;
                    break;
                }

                if (d > num15)
                {
                    index = i;
                    num15 = d;
                }

                if (Math.Abs(d - num13) > Epsilon)
                {
                    continue;
                }

                index = i;
                break;
            }

            return index;
        }

        /// <summary>
        /// The get point array.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="placement">
        /// The placement.
        /// </param>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see><cref>Point[]</cref></see>.
        /// </returns>
        private static Point[] GetPointArray(
            IList<Point> target, PlacementMode placement, Rect plugin, double width, double height)
        {
            Point[] pointArray;
            switch (placement)
            {
                case PlacementMode.Bottom:
                    pointArray = new[]
                                     {
                                         new Point(target[2].X, Math.Max(0.0, target[2].Y + 1.0)), 
                                         new Point((target[3].X - width) + 1.0, Math.Max(0.0, target[2].Y + 1.0)), 
                                         new Point(0.0, Math.Max(0.0, target[2].Y + 1.0))
                                     };
                    break;

                case PlacementMode.Right:
                    pointArray = new[]
                                     {
                                         new Point(Math.Max(0.0, target[1].X + 1.0), target[1].Y), 
                                         new Point(Math.Max(0.0, target[3].X + 1.0), (target[3].Y - height) + 1.0), 
                                         new Point(Math.Max(0.0, target[1].X + 1.0), 0.0)
                                     };
                    break;

                case PlacementMode.Left:
                    pointArray = new[]
                                     {
                                         new Point(Math.Min(plugin.Width, target[0].X) - width, target[1].Y), 
                                         new Point(
                                             Math.Min(plugin.Width, target[2].X) - width, (target[3].Y - height) + 1.0), 
                                         new Point(Math.Min(plugin.Width, target[0].X) - width, 0.0)
                                     };
                    break;

                case PlacementMode.Top:
                    pointArray = new[]
                                     {
                                         new Point(target[0].X, Math.Min(target[0].Y, plugin.Height) - height), 
                                         new Point(
                                             (target[1].X - width) + 1.0, Math.Min(target[0].Y, plugin.Height) - height), 
                                         new Point(0.0, Math.Min(target[0].Y, plugin.Height) - height)
                                     };
                    break;

                default:
                    pointArray = new[] { new Point(0.0, 0.0) };
                    break;
            }

            return pointArray;
        }

        /// <summary>
        /// The get translated points.
        /// </summary>
        /// <param name="frameworkElement">
        /// The framework element.
        /// </param>
        /// <returns>
        /// The <see><cref>Point[]</cref></see>.
        /// </returns>
        private static Point[] GetTranslatedPoints(FrameworkElement frameworkElement)
        {
            var pointArray = new Point[4];

            var tooltip = frameworkElement as PinnableTooltip;
            if (tooltip == null || tooltip.IsOpen)
            {
                GeneralTransform generalTransform =
                    frameworkElement.TransformToVisual(
                        tooltip != null ? tooltip.adornerLayer : PinnableTooltipService.RootVisual);
                pointArray[0] = generalTransform.Transform(new Point(0.0, 0.0));
                pointArray[1] = generalTransform.Transform(new Point(frameworkElement.ActualWidth, 0.0));
                pointArray[1].X--;
                pointArray[2] = generalTransform.Transform(new Point(0.0, frameworkElement.ActualHeight));
                pointArray[2].Y--;
                pointArray[3] =
                    generalTransform.Transform(new Point(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
                pointArray[3].X--;
                pointArray[3].Y--;
            }

            return pointArray;
        }

        /// <summary>
        /// The on horizontal offset property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tooltip = (PinnableTooltip)d;
            if (tooltip.IsOpen)
            {
                tooltip.PerformPlacement();
            }
        }

        /// <summary>
        /// The on is pinned property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnIsPinnedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tooltip = (PinnableTooltip)d;

            if ((bool)e.NewValue)
            {
                if (tooltip.adornerDragDrop == null && tooltip.adorner != null)
                {
                    tooltip.adornerDragDrop = ControlAdornerDragDrop.Attach(tooltip.adorner);
                }

                if (tooltip.IsTimerEnabled)
                {
                    tooltip.timer.StopAndReset();
                }
            }
            else
            {
                if (tooltip.adornerDragDrop != null)
                {
                    ControlAdornerDragDrop.Detach(tooltip.adornerDragDrop);
                    tooltip.adornerDragDrop = null;
                }

                tooltip.IsOpen = false;
            }
        }

        /// <summary>
        /// The on vertical offset property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tooltip = (PinnableTooltip)d;
            if (tooltip.IsOpen)
            {
                tooltip.PerformPlacement();
            }
        }

        /// <summary>
        /// The on grip color changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnGripColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tooltip = (PinnableTooltip)d;
            if (tooltip.gripDrawing != null)
            {
                tooltip.gripDrawing.Brush = new SolidColorBrush(tooltip.GripColor);
            }
        }

        /// <summary>
        /// The place popup.
        /// </summary>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="toolTip">
        /// The tool tip.
        /// </param>
        /// <param name="placement">
        /// The placement.
        /// </param>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        private static Point PlacePopup(Rect plugin, Point[] target, Point[] toolTip, PlacementMode placement)
        {
            Rect bounds = GetBounds(target);
            Rect rect2 = GetBounds(toolTip);
            double width = rect2.Width;
            double height = rect2.Height;

            placement = ValidatePlacement(target, placement, plugin, width, height);

            Point[] pointArray = GetPointArray(target, placement, plugin, width, height);
            int index = GetIndex(plugin, width, height, pointArray);
            Point point = CalculatePoint(target, placement, plugin, width, height, pointArray, index, bounds);

            return point;
        }

        /// <summary>
        /// The validate placement.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="placement">
        /// The placement.
        /// </param>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <returns>
        /// The <see cref="PlacementMode"/>.
        /// </returns>
        private static PlacementMode ValidatePlacement(
            IList<Point> target, PlacementMode placement, Rect plugin, double width, double height)
        {
            switch (placement)
            {
                case PlacementMode.Right:
                    double num5 = Math.Max(0.0, target[0].X - 1.0);
                    double num6 = plugin.Width - Math.Min(plugin.Width, target[1].X + 1.0);
                    if ((num6 < width) && (num6 < num5))
                    {
                        placement = PlacementMode.Left;
                    }

                    break;
                case PlacementMode.Left:
                    double num7 = Math.Min(plugin.Width, target[1].X + width) - target[1].X;
                    double num8 = target[0].X - Math.Max(0.0, target[0].X - width);
                    if ((num8 < width) && (num8 < num7))
                    {
                        placement = PlacementMode.Right;
                    }

                    break;
                case PlacementMode.Top:
                    double num9 = target[0].Y - Math.Max(0.0, target[0].Y - height);
                    double num10 = Math.Min(plugin.Height, plugin.Height - height) - target[2].Y;
                    if ((num9 < height) && (num9 < num10))
                    {
                        placement = PlacementMode.Bottom;
                    }

                    break;
                case PlacementMode.Bottom:
                    double num11 = Math.Max(0.0, target[0].Y);
                    double num12 = plugin.Height - Math.Min(plugin.Height, target[2].Y);
                    if ((num12 < height) && (num12 < num11))
                    {
                        placement = PlacementMode.Top;
                    }

                    break;
            }

            return placement;
        }

        /// <summary>
        /// Pin and start dragging.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnDragGripPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsPinned)
            {
                this.IsPinned = true;
            }
        }

        /// <summary>
        /// The pinnable tooltip_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPinnableTooltipMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsOpen && !this.IsPinned)
            {
                this.timer.StopAndReset();
            }
        }

        /// <summary>
        /// The pinnable tooltip_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPinnableTooltipMouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsTimerEnabled && !this.IsPinned)
            {
                this.timer.Stop();
            }
        }

        /// <summary>
        ///     The perform placement.
        /// </summary>
        internal void PerformPlacement()
        {
            if (this.adornerLayer == null)
            {
                return;
            }

            this.adornerLayer.Update();
        }

        /// <summary>
        /// The set owner.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        public void SetOwner(UIElement element)
        {
            this.owner = element;
        }

        /// <summary>
        /// The set user defined adorner.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        public void SetUserDefinedAdorner(UIElement element)
        {
            this.userDefinedAdorner = element;
        }

        /// <summary>
        /// Brings pinned toooltip to front.
        /// </summary>
        public void BringToFront()
        {
            if (this.IsPinned && this.adorner != null)
            {
                this.adornerLayer.Remove(this.adorner);

                var adornedElement = this.userDefinedAdorner != null ? this.userDefinedAdorner : Application.Current.MainWindow.Content as UIElement;
                if (adornedElement == null)
                {
                    return;
                }

                this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                if (this.adornerLayer == null)
                {
                    return;
                }

                this.adornerLayer.Add(this.adorner);
            }
        }

        /// <summary>
        /// The setup timer.
        /// </summary>
        /// <param name="initialShowDelay">
        /// The initial show delay.
        /// </param>
        /// <param name="showDuration">
        /// The show duration.
        /// </param>
        public void SetupTimer(int initialShowDelay, int showDuration)
        {
            if (this.timer != null)
            {
                if (this.timer.IsEnabled)
                {
                    this.timer.StopAndReset();
                }

                this.timer.Tick -= this.OnTimerTick;
                this.timer.Stopped -= this.OnTimerStopped;
            }

            this.timer = new TooltipTimer(
                TimeSpan.FromMilliseconds(showDuration), TimeSpan.FromMilliseconds(initialShowDelay));
            this.timer.Tick += this.OnTimerTick;
            this.timer.Stopped += this.OnTimerStopped;
        }

        /// <summary>
        ///     The start timer.
        /// </summary>
        public void StartTimer()
        {
            if (this.timer != null)
            {
                this.timer.StartAndReset();
            }
        }

        /// <summary>
        ///     The stop timer.
        /// </summary>
        public void StopTimer()
        {
            if (this.timer != null && this.IsTimerEnabled)
            {
                this.timer.StopAndReset();
            }
        }

        /// <summary>
        ///     The create adorner.
        /// </summary>
        private void CreateAdorner()
        {
            if (this.adorner != null || (Application.Current.MainWindow == null && this.userDefinedAdorner == null))
            {
                return;
            }

            var adornedElement = this.userDefinedAdorner != null ? this.userDefinedAdorner : Application.Current.MainWindow.Content as UIElement;
            if (adornedElement == null)
            {
                return;
            }

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(adornedElement);
            if (layer == null)
            {
                return;
            }

            this.isPositionCalculated = false;
            var ad = new ControlAdorner(adornedElement) { Child = this, Focusable = false };
            KeyboardNavigation.SetTabNavigation(ad, KeyboardNavigationMode.None);
            layer.Add(ad);
            this.adorner = ad;
            this.adornerLayer = layer;

            if (this.IsPinned && this.adornerDragDrop == null)
            {
                this.adornerDragDrop = ControlAdornerDragDrop.Attach(this.adorner);
            }
        }

        /// <summary>
        /// The on size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.lastSize = e.NewSize;
        }

        /// <summary>
        /// The on timer stopped.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTimerStopped(object sender, EventArgs e)
        {
            if (!this.IsPinned && !this.IsMouseOver)
            {
                this.IsOpen = false;
                this.lastPosition = new Point(0, 0);
            }
        }

        /// <summary>
        /// The on timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            if (this.IsOpen)
            {
                return;
            }

            if (this.timer.IsEnabled && this.timer.MaximumTicks.TotalMilliseconds > 0
                && this.timer.CurrentTick >= this.timer.InitialDelay.TotalMilliseconds)
            {
                this.IsOpen = true;
            }
        }

        /// <summary>
        ///     The remove adorner.
        /// </summary>
        private void RemoveAdorner()
        {
            if (this.adorner == null || this.adornerLayer == null)
            {
                return;
            }

            if (this.adornerDragDrop != null)
            {
                ControlAdornerDragDrop.Detach(this.adornerDragDrop);
                this.adornerDragDrop = null;
            }

            this.adornerLayer.Remove(this.adorner);
            this.adorner.Child = null;
            this.adorner = null;
            this.adornerLayer = null;
        }

        #endregion
    }
}