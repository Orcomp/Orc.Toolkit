// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PinnableTooltip.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using Orc.Toolkit.TooltipControls.Helpers;

    [TemplatePart(Name = "PinButton", Type = typeof(ToggleButton))]
    public class PinnableTooltip : ContentControl, IControlAdornerChild
    {
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(
                "HorizontalOffset",
                typeof(double),
                typeof(PinnableTooltip),
                new PropertyMetadata(OnHorizontalOffsetPropertyChanged));

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
            "VerticalOffset",
            typeof(double),
            typeof(PinnableTooltip),
            new PropertyMetadata(OnVerticalOffsetPropertyChanged));

        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
            "IsPinned", typeof(bool), typeof(PinnableTooltip), new PropertyMetadata(false, OnIsPinnedPropertyChanged));

        private const double Epsilon = 1E-7;

        private ControlAdorner adorner;

        private AdornerLayer adornerLayer;

        private UIElement owner;

        private TooltipTimer timer;

        private Size lastSize;

        private ControlAdornerDragDrop adornerDragDrop;

        private Point lastPosition;

        private bool isPositionCalculated;

        public PinnableTooltip()
        {
            this.DefaultStyleKey = typeof(PinnableTooltip);
            this.SizeChanged += this.OnSizeChanged;
        }

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

        public double HorizontalOffset
        {
            get
            {
                return (double)GetValue(HorizontalOffsetProperty);
            }

            set
            {
                this.SetValue(HorizontalOffsetProperty, value);
            }
        }

        public double VerticalOffset
        {
            get
            {
                return (double)GetValue(VerticalOffsetProperty);
            }

            set
            {
                this.SetValue(VerticalOffsetProperty, value);
            }
        }

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

        internal bool IsTimerEnabled
        {
            get
            {
                return this.timer != null && this.timer.IsEnabled;
            }
        }

        public Point GetPosition()
        {
            var position = new Point();

            if (this.owner == null)
            {
                return position;
            }

            var horizontalOffset = this.HorizontalOffset;
            var verticalOffset = this.VerticalOffset;

            var placementMode = PinnableTooltipService.GetPlacement(this.owner);
            var placementTarget = PinnableTooltipService.GetPlacementTarget(this.owner) ?? this.owner;

            var mousePosition = PinnableTooltipService.MousePosition;
            var rootVisual = PinnableTooltipService.RootVisual;

            switch (placementMode)
            {
                case PlacementMode.Mouse:
                    if (this.isPositionCalculated)
                    {
                        position = this.lastPosition;
                        return position;
                    }

                    var offsetX = mousePosition.X + horizontalOffset;
                    var offsetY = mousePosition.Y + new TextBlock().FontSize + verticalOffset;

                    offsetX = Math.Max(2.0, offsetX);
                    offsetY = Math.Max(2.0, offsetY);

                    var actualHeight = rootVisual.ActualHeight;
                    var actualWidth = rootVisual.ActualWidth;
                    var lastHeight = this.lastSize.Height;
                    var lastWidth = this.lastSize.Width;

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
                    var windowSize = ScreenUtils.GetWindowSize();
                    var plugin = new Rect(0.0, 0.0, windowSize.Width, windowSize.Height);
                    var translatedPoints = GetTranslatedPoints((FrameworkElement)placementTarget);
                    var toolTipPoints = GetTranslatedPoints(this);
                    var popupLocation = PlacePopup(plugin, translatedPoints, toolTipPoints, placementMode);

                    position.Y = popupLocation.Y + verticalOffset;
                    position.X = popupLocation.X + horizontalOffset;
                    break;
            }

            return position;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var toggleButton = this.GetTemplateChild("PinButton") as ToggleButton;
            if (toggleButton != null)
            {
                toggleButton.Focusable = false;
            }
        }

        internal void SetOwner(UIElement element)
        {
            this.owner = element;
        }

        internal void SetupTimer(int initialShowDelay, int showDuration)
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

        internal void StartTimer()
        {
            if (this.timer != null)
            {
                this.timer.StartAndReset();
            }
        }

        internal void StopTimer()
        {
            if (this.timer != null && this.IsTimerEnabled)
            {
                this.timer.StopAndReset();
            }
        }

        internal void PerformPlacement()
        {
            if (this.adornerLayer == null)
            {
                return;
            }

            this.adornerLayer.Update();
        }

        private static Point[] GetTranslatedPoints(FrameworkElement frameworkElement)
        {
            var pointArray = new Point[4];

            var tooltip = frameworkElement as PinnableTooltip;
            if (tooltip == null || tooltip.IsOpen)
            {
                var generalTransform = frameworkElement.TransformToVisual(tooltip != null ? tooltip.adornerLayer : PinnableTooltipService.RootVisual);
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

        private static Point PlacePopup(Rect plugin, Point[] target, Point[] toolTip, PlacementMode placement)
        {
            var bounds = GetBounds(target);
            var rect2 = GetBounds(toolTip);
            var width = rect2.Width;
            var height = rect2.Height;

            placement = ValidatePlacement(target, placement, plugin, width, height);

            var pointArray = GetPointArray(target, placement, plugin, width, height);
            var index = GetIndex(plugin, width, height, pointArray);
            var point = CalculatePoint(target, placement, plugin, width, height, pointArray, index, bounds);

            return point;
        }

        private static int GetIndex(Rect plugin, double width, double height, IList<Point> pointArray)
        {
            var num13 = width * height;
            var index = 0;
            var num15 = 0.0;
            for (var i = 0; i < pointArray.Count; i++)
            {
                var rect3 = new Rect(pointArray[i].X, pointArray[i].Y, width, height);
                rect3.Intersect(plugin);
                var d = rect3.Width * rect3.Height;
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
            var x = pointArray[index].X;
            var y = pointArray[index].Y;
            if (index > 1)
            {
                if ((placement == PlacementMode.Left) || (placement == PlacementMode.Right))
                {
                    if (((Math.Abs(y - target[0].Y) > Epsilon) && (Math.Abs(y - target[1].Y) > Epsilon))
                        && ((Math.Abs((y + height) - target[0].Y) > Epsilon)
                            && (Math.Abs((y + height) - target[1].Y) > Epsilon)))
                    {
                        var num18 = bounds.Top + (bounds.Height / 2.0);
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
                         && (((x != target[0].X) && (x != target[1].X))
                             && (((x + width) != target[0].X) && ((x + width) != target[1].X))))
                {
                    var num19 = bounds.Left + (bounds.Width / 2.0);
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

        private static PlacementMode ValidatePlacement(
            IList<Point> target, PlacementMode placement, Rect plugin, double width, double height)
        {
            switch (placement)
            {
                case PlacementMode.Right:
                    var num5 = Math.Max(0.0, target[0].X - 1.0);
                    var num6 = plugin.Width - Math.Min(plugin.Width, target[1].X + 1.0);
                    if ((num6 < width) && (num6 < num5))
                    {
                        placement = PlacementMode.Left;
                    }

                    break;
                case PlacementMode.Left:
                    var num7 = Math.Min(plugin.Width, target[1].X + width) - target[1].X;
                    var num8 = target[0].X - Math.Max(0.0, target[0].X - width);
                    if ((num8 < width) && (num8 < num7))
                    {
                        placement = PlacementMode.Right;
                    }

                    break;
                case PlacementMode.Top:
                    var num9 = target[0].Y - Math.Max(0.0, target[0].Y - height);
                    var num10 = Math.Min(plugin.Height, plugin.Height - height) - target[2].Y;
                    if ((num9 < height) && (num9 < num10))
                    {
                        placement = PlacementMode.Bottom;
                    }

                    break;
                case PlacementMode.Bottom:
                    var num11 = Math.Max(0.0, target[0].Y);
                    var num12 = plugin.Height - Math.Min(plugin.Height, target[2].Y);
                    if ((num12 < height) && (num12 < num11))
                    {
                        placement = PlacementMode.Top;
                    }

                    break;
            }

            return placement;
        }

        private static Rect GetBounds(params Point[] interestPoints)
        {
            double num2;
            double num4;
            var x = num2 = interestPoints[0].X;
            var y = num4 = interestPoints[0].Y;
            for (var i = 1; i < interestPoints.Length; i++)
            {
                var num6 = interestPoints[i].X;
                var num7 = interestPoints[i].Y;
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

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tooltip = (PinnableTooltip)d;
            if (tooltip.IsOpen)
            {
                tooltip.PerformPlacement();
            }
        }

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tooltip = (PinnableTooltip)d;
            if (tooltip.IsOpen)
            {
                tooltip.PerformPlacement();
            }
        }

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

        private void OnTimerStopped(object sender, EventArgs e)
        {
            if (!this.IsPinned)
            {
                this.IsOpen = false;
            }
        }

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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.lastSize = e.NewSize;
        }

        private void PerformClipping(Size size)
        {
            var child = VisualTreeHelper.GetChild(this, 0) as Border;
            if (child == null)
            {
                return;
            }

            if (size.Width < child.ActualWidth)
            {
                child.Width = size.Width;
            }

            if (size.Height < child.ActualHeight)
            {
                child.Height = size.Height;
            }
        }

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

        private void CreateAdorner()
        {
            if (this.adorner != null || Application.Current.MainWindow == null)
            {
                return;
            }

            var adornedElement = Application.Current.MainWindow.Content as UIElement;
            if (adornedElement == null)
            {
                return;
            }

            var layer = AdornerLayer.GetAdornerLayer(adornedElement);
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
    }
}
