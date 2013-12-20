// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailabilityDropDown.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// The availability drop down.
    /// </summary>
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_AvailabilityIndicator", Type = typeof(AvailabilityIndicator))]
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_DragGrip", Type = typeof(FrameworkElement))]
    public class AvailabilityDropDown : HeaderedContentControl
    {
        #region fields
        /// <summary>
        /// last mouse position
        /// </summary>
        private Point lastMousePosition;

        /// <summary>
        /// is mouse draging
        /// </summary>
        private bool isDraging;

        /// <summary>
        /// The drag grip.
        /// </summary>
        private FrameworkElement dragGrip;

        /// <summary>
        /// The content.
        /// </summary>
        private ContentControl content;

        /// <summary>
        /// The popup.
        /// </summary>
        private Popup popup;

        /// <summary>
        /// The open on window activation.
        /// </summary>
        private bool openOnWindowActivation;

        /// <summary>
        /// The core indicator.
        /// </summary>
        private AvailabilityIndicator indicator;
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AvailabilityDropDown"/> class.
        /// </summary>
        public AvailabilityDropDown()
        {
            this.DefaultStyleKey = typeof(AvailabilityDropDown);
        }
        #endregion

        #region DP

        /// <summary>
        /// The availability property.
        /// </summary>
        public static readonly DependencyProperty IsAvailableProperty = DependencyProperty.Register(
            "IsAvailable",
            typeof(bool),
            typeof(AvailabilityIndicator),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsAvailableChanged)));

        /// <summary>
        /// The is in use property.
        /// </summary>
        public static readonly DependencyProperty IsInUseProperty = DependencyProperty.Register(
            "IsInUse",
            typeof(bool),
            typeof(AvailabilityIndicator),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsInUseChanged)));

        /// <summary>
        /// The in use period property.
        /// </summary>
        public static readonly DependencyProperty InUsePeriodProperty = DependencyProperty.Register(
            "InUsePeriod",
            typeof(double),
            typeof(AvailabilityIndicator),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnInUsePeriodChanged)));

        /// <summary>
        /// The is dragable property.
        /// </summary>
        public static readonly DependencyProperty IsDragableProperty = DependencyProperty.Register(
            "IsDragable", 
            typeof(bool), 
            typeof(AvailabilityDropDown), 
            new PropertyMetadata(false));

        /// <summary>
        /// The is pinned property.
        /// </summary>
        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
            "IsPinned",
            typeof(bool),
            typeof(AvailabilityDropDown),
            new PropertyMetadata(false, new PropertyChangedCallback(OnPinnedPropertyChanged)));

        #if(SILVERLIGHT)
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(AvailabilityDropDown), new PropertyMetadata(PlacementMode.Bottom));
        #endif

        #endregion

        /// <summary>
        /// The popup opened.
        /// </summary>
        public event EventHandler PopupOpened;

        /// <summary>
        /// The content layout updated.
        /// </summary>
        public event EventHandler ContentLayoutUpdated;

        /// <summary>
        /// Gets or sets a value indicating whether service is available.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return (bool)this.GetValue(IsAvailableProperty);
            }

            set
            {
                this.SetValue(IsAvailableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether service is available.
        /// </summary>
        public bool IsInUse
        {
            get
            {
                return (bool)this.GetValue(IsInUseProperty);
            }

            set
            {
                System.Diagnostics.Debug.WriteLine("IsInUse new value = {0}", value);
                this.SetValue(IsInUseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the in use period.
        /// </summary>
        public double InUsePeriod
        {
            get
            {
                return (double)this.GetValue(InUsePeriodProperty);
            }

            set
            {
                this.SetValue(InUsePeriodProperty, value);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether is pinned.
        /// </summary>
        public bool IsPinned
        {
            get { return (bool)this.GetValue(IsPinnedProperty); }
            set { this.SetValue(IsPinnedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is dragable.
        /// </summary>
        public bool IsDragable
        {
            get { return (bool)this.GetValue(IsDragableProperty); }
            set { this.SetValue(IsDragableProperty, value); }
        }

        #if(SILVERLIGHT)
        public PlacementMode PopupPlacement
        {
            get
            {
                return (PlacementMode)this.GetValue(PopupPlacementProperty);
            }

            set
            {
                this.SetValue(PopupPlacementProperty, value);
            }
        }
        #endif

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.indicator = (AvailabilityIndicator)this.GetTemplateChild("PART_AvailabilityIndicator");
            this.popup = (Popup)this.GetTemplateChild("PART_Popup");
            this.content = (ContentControl)this.GetTemplateChild("PART_Content");
            this.dragGrip = (FrameworkElement)this.GetTemplateChild("PART_DragGrip");

            if (this.popup != null)
            {
                this.popup.Opened += (sender, args) =>
                {
                    if (this.PopupOpened != null)
                    {
                        this.PopupOpened(sender, args);
                    }
                };
            }

            if (this.content != null)
            {
                this.content.LayoutUpdated += (sender, args) =>
                {
                    if (this.ContentLayoutUpdated != null)
                    {
                        this.ContentLayoutUpdated(this.content, args);
                    }
                };
            }

            this.SizeChanged += this.AvailabilityDropDownSizeChanged;
            if (this.dragGrip != null)
            {
                this.dragGrip.MouseLeftButtonDown += this.DragGripMouseLeftButtonDown;
                this.dragGrip.MouseMove += this.DragGripMouseMove;
                this.dragGrip.MouseLeftButtonUp += this.DragGripMouseLeftButtonUp;
            }

#if(SILVERLIGHT)
            UIElement root = Application.Current.RootVisual;
            if (root != null)
            {
                root.MouseLeftButtonDown += (s, ee) =>
                {
                    if (this.popup.IsOpen && !this.IsPinned)
                        this.popup.IsOpen = false;
                };
            }
#endif

#if (!SILVERLIGHT)
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Window window = Window.GetWindow(this);
                if (window != null)
                {
                    window.LocationChanged += this.WindowLocationChanged;
                    window.SizeChanged += this.WindowSizeChanged;
                    window.Deactivated += this.WindowDeactivated;
                    window.Activated += this.WindowActivated;
                }

                this.LayoutUpdated += this.AvailabilityDropDownLayoutUpdated;
            }

            var popup1 = this.popup;
            if (popup1 != null)
            {
                popup1.Opened += this.OnPopupOpened;
            }

            var curWindow = Window.GetWindow(this);
            if (curWindow != null)
            {
                curWindow.AddHandler(MouseDownEvent, new MouseButtonEventHandler(this.OutsideMouseDown), true);
                curWindow.Activated += this.WindowActivated;
            }
#endif
        }

        /// <summary>
        /// The on pinned property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnPinnedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var availabilityDropDown = d as AvailabilityDropDown;

#if(!SILVERLIGHT)
            
            if (availabilityDropDown != null)
            {
                if ((bool)e.NewValue == true)
                {
                    availabilityDropDown.popup.StaysOpen = true;
                }
                else
                {
                    availabilityDropDown.popup.StaysOpen = false;                    
                    availabilityDropDown.popup.IsOpen = false;
                    availabilityDropDown.popup.HorizontalOffset = availabilityDropDown.popup.VerticalOffset = 0;
                }
            }

#else
            if (availabilityDropDown != null)
            {
                if ((bool)e.NewValue != true)
                {
                    availabilityDropDown.popup.IsOpen = false;
                    availabilityDropDown.UpdatePopupPosition();
                }
            }
#endif
        }

        /// <summary>
        /// The on in use period changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnInUsePeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AvailabilityDropDown availabilityDropDown = d as AvailabilityDropDown;
            if (availabilityDropDown != null)
            {
                availabilityDropDown.indicator.InUsePeriod = (double)e.NewValue;
            }
        }

        /// <summary>
        /// The on is in use changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnIsInUseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AvailabilityDropDown availabilityDropDown = d as AvailabilityDropDown;
            if (availabilityDropDown != null)
            {
                availabilityDropDown.indicator.IsInUse = (bool)e.NewValue;
            }
        }

        /// <summary>
        /// The on availability changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnIsAvailableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AvailabilityDropDown availabilityDropDown = d as AvailabilityDropDown;
            if (availabilityDropDown != null)
            {
                availabilityDropDown.indicator.IsAvailable = (bool) e.NewValue;
            }
        }

        /// <summary>
        /// The drag grip_ mouse left button up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DragGripMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dragGrip.ReleaseMouseCapture();
            this.isDraging = false;
        }

        /// <summary>
        /// The drag grip_ mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DragGripMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDraging)
            {
#if(!SILVERLIGHT)
                Point currentMousePosition = Window.GetWindow(this).PointToScreen(e.GetPosition(Window.GetWindow(this)));
#else
                Point currentMousePosition = e.GetPosition(Application.Current.RootVisual);
#endif

                this.popup.HorizontalOffset += currentMousePosition.X - this.lastMousePosition.X;
                this.popup.VerticalOffset += currentMousePosition.Y - this.lastMousePosition.Y;                

                this.lastMousePosition = currentMousePosition;
            }
        }

        /// <summary>
        /// The drag grip_ mouse left button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DragGripMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isDraging = this.dragGrip.CaptureMouse();
            if (this.isDraging)
            {
                this.IsPinned = true;
#if(!SILVERLIGHT)
                var relativeTo = Window.GetWindow(this);
                if (relativeTo != null)
                {
                    this.lastMousePosition = relativeTo.PointToScreen(e.GetPosition(relativeTo));
                }
#else
                lastMousePosition = e.GetPosition(Application.Current.RootVisual);
#endif
            }
        }


#if (!SILVERLIGHT)
        /// <summary>
        /// The outside_ mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OutsideMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.popup.IsOpen && !this.IsPinned)
            {
                Point p = e.GetPosition(this.popup);
                if (this.popup.Child != null)
                {
                    Point p2 = e.GetPosition(this.popup.Child);
                    if ((p2.Y > 0) && (p2.X > 0))
                    {
                        p = p2;
                    }
                }

                var frameworkElement = this.popup.Child as FrameworkElement;

                if (frameworkElement != null
                    && !new Rect(0, 0, frameworkElement.ActualWidth, frameworkElement.ActualHeight).Contains(p))
                {
                    this.popup.IsOpen = false;
                }
            }
        }

        /// <summary>
        /// The window_ deactivated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WindowDeactivated(object sender, EventArgs e)
        {
            if (this.popup.IsOpen)
            {
                if (!Window.GetWindow(this).IsActive)
                {
                    if (this.popup.Child != null)
                    {
                        this.openOnWindowActivation = true;
                        this.popup.Child.IsHitTestVisible = false;
                    }
                }

                this.popup.IsOpen = false;
            }
        }

        /// <summary>
        /// The window_ activated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WindowActivated(object sender, EventArgs e)
        {
            if (this.openOnWindowActivation)
            {
                this.openOnWindowActivation = false;
                this.popup.IsOpen = true;
                this.popup.Child.IsHitTestVisible = true;
            }
        }

        /// <summary>
        /// The popup_ opened.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPopupOpened(object sender, EventArgs e)
        {
            if (this.popup.Child != null)
            {
                this.popup.Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else
            {
                this.popup.Focus();
            }
        }

        /// <summary>
        /// The window_ size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePopupPosition();
        }

        /// <summary>
        /// The window_ location changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void WindowLocationChanged(object sender, EventArgs e)
        {
            this.UpdatePopupPosition();
        }

        /// <summary>
        /// The drop down button_ layout updated.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AvailabilityDropDownLayoutUpdated(object sender, EventArgs e)
        {
            this.UpdatePopupPosition();
        }

#endif

        /// <summary>
        /// The update popup position.
        /// </summary>
        private void UpdatePopupPosition()
        {
            if (this.IsPinned)
            {
                return;
            }

#if(!SILVERLIGHT)
            if (this.popup != null)
            {
                if (this.popup.IsOpen)
                {
                    this.popup.HorizontalOffset += 0.1;
                    this.popup.HorizontalOffset -= 0.1;
                }
            }

#else
            if (this.PopupPlacement == PlacementMode.Bottom)
            {
                this.popup.VerticalOffset = this.ActualHeight;
                this.popup.HorizontalOffset = 0;
            }

            if (this.PopupPlacement == PlacementMode.Top)
            {
                this.popup.VerticalOffset = -1 * this.content.ActualHeight;
                this.popup.HorizontalOffset = 0;
            }

            if (this.PopupPlacement == PlacementMode.Right)
            {
                this.popup.HorizontalOffset = this.ActualWidth;
                this.popup.VerticalOffset = 0;
            }

            if (this.PopupPlacement == PlacementMode.Left)
            {
                this.popup.HorizontalOffset = -1 * this.content.ActualWidth;
                this.popup.VerticalOffset = 0;
            }
#endif
        }

        /// <summary>
        /// The availability drop down size changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AvailabilityDropDownSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.popup != null)
            {
                this.UpdatePopupPosition();
            }
        }
    }
}
