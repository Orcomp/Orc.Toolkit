// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropDownButton.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The drop down button.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// The drop down button.
    /// </summary>
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ToggleDropDown", Type = typeof(ToggleButton))]    
    [TemplatePart(Name = "PART_Content", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_DragGrip", Type = typeof(FrameworkElement))]
    public class DropDownButton : HeaderedContentControl
    {
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

        private bool openOnWindowActivation = false;

        /// <summary>
        /// The button.
        /// </summary>
        private ToggleButton button;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownButton"/> class.
        /// </summary>
        public DropDownButton()
        {
            this.DefaultStyleKey = typeof(DropDownButton);
        }

        /// <summary>
        /// The popup opened.
        /// </summary>
        public event EventHandler PopupOpened;

        /// <summary>
        /// The content layout updated.
        /// </summary>
        public event EventHandler ContentLayoutUpdated;

        #region OVERRIDE

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.button = (ToggleButton)this.GetTemplateChild("PART_ToggleDropDown");
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
                        
            this.SizeChanged += this.DropDownButton_SizeChanged;
            if (this.dragGrip != null)
            {
                this.dragGrip.MouseLeftButtonDown += this.dragGrip_MouseLeftButtonDown;
                this.dragGrip.MouseMove += this.dragGrip_MouseMove;
                this.dragGrip.MouseLeftButtonUp += this.dragGrip_MouseLeftButtonUp;
            }

#if(SILVERLIGHT)
            UIElement root = Application.Current.RootVisual;
            if (root != null)
            {
                root.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.OnRootMouseLeftButtonDown), true);
            }
#endif

#if (!SILVERLIGHT)
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                System.Windows.Window window = System.Windows.Window.GetWindow(this);
                window.LocationChanged += window_LocationChanged;
                window.SizeChanged += window_SizeChanged;
                window.Deactivated += window_Deactivated;
                window.Activated += window_Activated;
                LayoutUpdated += DropDownButton_LayoutUpdated;
            }

            popup.Opened += popup_Opened;
            Window.GetWindow(this).AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(Outside_MouseDown), true);
            Window.GetWindow(this).Activated += window_Activated;
#endif
        }

        /// <summary>
        /// The on root mouse left button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnRootMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.popup.IsOpen || this.IsPinned || (this.popup.Child as FrameworkElement) == null)
            {
                return;
            }

            Point p = e.GetPosition(this.popup);

            Point p2 = e.GetPosition(this.popup.Child);
            if ((p2.Y > 0) && (p2.X > 0))
            {
                p = p2;
            }

            double width = (this.popup.Child as FrameworkElement).ActualWidth;
            double height = (this.popup.Child as FrameworkElement).ActualHeight;

            if ((width < 5) || (height < 5))
            {
                return;
            }

            if (!new Rect(0, 0, width, height).Contains(p))
            {
                this.popup.IsOpen = false;
            }
        }

        #endregion

        #region DP
        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.Register("IsPinned", typeof(bool), typeof(DropDownButton), new PropertyMetadata(false,
                new PropertyChangedCallback(OnPinnedPropertyChanged)));
        private static void OnPinnedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton dropDownButton = d as DropDownButton;
#if(!SILVERLIGHT)
            
            if (dropDownButton != null)
            {
                if ((bool)e.NewValue == true)
                {
                    dropDownButton.popup.StaysOpen = true;
                }
                else
                {
                    dropDownButton.popup.StaysOpen = false;                    
                    dropDownButton.popup.IsOpen = false;
                    dropDownButton.popup.HorizontalOffset = dropDownButton.popup.VerticalOffset = 0;
                }
            }
#else
            if (dropDownButton != null)
            {
                if ((bool)e.NewValue != true)
                {
                    dropDownButton.popup.IsOpen = false;
                    dropDownButton.UpdatePopupPosition();
                }
            }
#endif
        }
        
        public bool IsDragable
        {
            get { return (bool)GetValue(IsDragableProperty); }
            set { SetValue(IsDragableProperty, value); }
        }
        public static readonly DependencyProperty IsDragableProperty =
            DependencyProperty.Register("IsDragable", typeof(bool), typeof(DropDownButton), new PropertyMetadata(false));
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
        public static readonly DependencyProperty PopupPlacementProperty =
            DependencyProperty.Register("PopupPlacement", typeof(PlacementMode), typeof(DropDownButton), new PropertyMetadata(PlacementMode.Bottom));
#endif
        #endregion

        #region private

        void dragGrip_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.dragGrip.ReleaseMouseCapture();
            this.isDraging = false;
        }


        void dragGrip_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDraging)
            {
#if(!SILVERLIGHT)
                Point currentMousePosition = System.Windows.Window.GetWindow(this).PointToScreen(e.GetPosition(System.Windows.Window.GetWindow(this)));
#else
                Point currentMousePosition = e.GetPosition(Application.Current.RootVisual);
#endif

                this.popup.HorizontalOffset += (currentMousePosition.X - lastMousePosition.X);
                this.popup.VerticalOffset += (currentMousePosition.Y - lastMousePosition.Y);                

                this.lastMousePosition = currentMousePosition;
            }
        }

        void dragGrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDraging = this.dragGrip.CaptureMouse();
            if (isDraging)
            {
                IsPinned = true;
#if(!SILVERLIGHT)
                lastMousePosition = System.Windows.Window.GetWindow(this).PointToScreen(e.GetPosition(System.Windows.Window.GetWindow(this)));
#else
                lastMousePosition = e.GetPosition(Application.Current.RootVisual);
#endif
            }
        }


#if (!SILVERLIGHT)
        void Outside_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.popup.IsOpen && !this.IsPinned)
            {
                Point p = e.GetPosition(popup);
                if (popup.Child != null)
                {
                    Point p2 = e.GetPosition(popup.Child);
                    if ((p2.Y > 0) && (p2.X > 0))
                    {
                        p = p2;
                    }
                }
                if (!new Rect(0, 0, (popup.Child as FrameworkElement).ActualWidth, (popup.Child as FrameworkElement).ActualHeight).Contains(p))
                    popup.IsOpen = false;
            }
        }

        void window_Deactivated(object sender, EventArgs e)
        {
            if (this.popup.IsOpen)
            {
                if (!Window.GetWindow(this).IsActive)
                {
                    if (popup.Child != null)
                    {
                        openOnWindowActivation = true;
                        popup.Child.IsHitTestVisible = false;
                    }

                }

                popup.IsOpen = false;
            }
        }

        void window_Activated(object sender, EventArgs e)
        {
            if (openOnWindowActivation)
            {
                openOnWindowActivation = false;
                popup.IsOpen = true;
                popup.Child.IsHitTestVisible = true;
            }
        }
#endif

#if (!SILVERLIGHT)
        void popup_Opened(object sender, EventArgs e)
        {
            if (this.popup.Child != null)
                this.popup.Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            else
                this.popup.Focus();
        }
        
        void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.UpdatePopupPosition();
        }

        void window_LocationChanged(object sender, EventArgs e)
        {
            this.UpdatePopupPosition();
        }

        void DropDownButton_LayoutUpdated(object sender, EventArgs e)
        {
            this.UpdatePopupPosition();
        }
#endif
        void UpdatePopupPosition()
        {
            if (this.IsPinned)
                return;
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

        private void DropDownButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (popup != null)
            {
                this.UpdatePopupPosition();
            }
        }

        #endregion
    }
}