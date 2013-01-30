// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PinnableTooltipService.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.TooltipControls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

#if (!SILVERLIGHT)
    using System.Windows.Interop;
    using System.Windows.Media;
    using Orc.Toolkit.TooltipControls.Helpers;
#endif

    public static class PinnableTooltipService
    {
        public static readonly DependencyProperty TooltipProperty = 
            DependencyProperty.RegisterAttached(
            "Tooltip", 
            typeof(object),
            typeof(PinnableTooltipService),
            new PropertyMetadata(new PropertyChangedCallback(OnTooltipPropertyChanged)));

        public static readonly DependencyProperty ShowDurationProperty =
            DependencyProperty.RegisterAttached(
            "ShowDurartion", 
            typeof(int), 
            typeof(PinnableTooltipService),
            new PropertyMetadata(DefaultShowDuration));

        public static readonly DependencyProperty InitialShowDelayProperty =
            DependencyProperty.RegisterAttached(
            "InitialShowDelay",
            typeof(int),
            typeof(PinnableTooltipService),
            new PropertyMetadata(DefaultInitialShowDelay));

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.RegisterAttached(
            "Placement",
            typeof(PlacementMode),
            typeof(PinnableTooltipService),
            new PropertyMetadata(PlacementMode.Mouse, OnPlacementPropertyChanged));

        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.RegisterAttached(
            "PlacementTarget",
            typeof(UIElement),
            typeof(PinnableTooltipService),
            new PropertyMetadata(OnPlacementTargetPropertyChanged));

        private const int DefaultShowDuration = 5000;
        private const int DefaultInitialShowDelay = 500;
        private static readonly Dictionary<UIElement, PinnableTooltip> ElementsAndToolTips = new Dictionary<UIElement, PinnableTooltip>();
        private static readonly object Locker = new object();
        private static FrameworkElement rootVisual;
        private static Point mousePosition;
        
        internal static Point MousePosition 
        { 
            get
            {
                lock (Locker)
                {
                    return mousePosition;
                }
            } 

            private set
            {
                lock (Locker)
                {
                    mousePosition = value;
                }
            }
        }

        internal static FrameworkElement RootVisual
        {
            get
            {
                lock (Locker)
                {
                    return rootVisual;
                }
            }

            private set
            {
                lock (Locker)
                {
                    rootVisual = value;
                }
            }
        }

        public static object GetTooltip(DependencyObject element)
        {
            return element.GetValue(TooltipProperty);
        }

        public static void SetTooltip(DependencyObject element, object value)
        {
            element.SetValue(TooltipProperty, value);
        }

        public static int GetShowDuration(DependencyObject element)
        {
            return (int)element.GetValue(ShowDurationProperty);
        }

        public static void SetShowDuration(DependencyObject element, int value)
        {
            element.SetValue(ShowDurationProperty, value);
        }

        public static int GetInitialShowDelay(DependencyObject element)
        {
            return (int)element.GetValue(InitialShowDelayProperty);
        }

        public static void SetInitialShowDelay(DependencyObject element, int value)
        {
            element.SetValue(InitialShowDelayProperty, value);
        }

        public static PlacementMode GetPlacement(DependencyObject element)
        {
            return (PlacementMode)element.GetValue(PlacementProperty);
        }

        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
           element.SetValue(PlacementProperty, value);
        }

        public static UIElement GetPlacementTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(PlacementTargetProperty);
        }

        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(PlacementTargetProperty, value);
        }

        private static void OnTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                UnregisterTooltip(d as UIElement);
            }

            if (e.NewValue != null)
            {
                RegisterTooltip(d as UIElement, e.NewValue);
            }
        }

        private static void OnPlacementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)d;
            
            PinnableTooltip tooltip = null;
            lock (Locker)
            {
                if (ElementsAndToolTips.ContainsKey(element))
                {
                    tooltip = ElementsAndToolTips[element];
                }
            }

            if (tooltip != null && tooltip.IsOpen)
            {
                tooltip.PerformPlacement();
            }
        }

        private static void OnPlacementTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (UIElement)d;
            
            PinnableTooltip tooltip = null;
            lock (Locker)
            {
                if (ElementsAndToolTips.ContainsKey(element))
                {
                    tooltip = ElementsAndToolTips[element];
                }
            }

            if (tooltip != null && tooltip.IsOpen)
            {
                tooltip.PerformPlacement();
            }
        }

        private static void UnregisterTooltip(UIElement owner)
        {
            if (owner == null)
            {
                return;
            }

            lock (Locker)
            {
                PinnableTooltip toolTip = null;
                if (ElementsAndToolTips.ContainsKey(owner))
                {
                    toolTip = ElementsAndToolTips[owner];
                }
                else
                {
                    return;
                }

                var element = owner as FrameworkElement;
                if (element != null)
                {
                    element.Unloaded -= FrameworkElementUnloaded;
                    toolTip.DataContext = null;
                }

                owner.MouseEnter -= OnElementMouseEnter;
                owner.MouseLeave -= OnElementMouseLeave;

                var control = owner as Control;
                if (control != null)
                {
                    control.IsEnabledChanged -= OnControlEnabledChanged;
                }

                toolTip.IsOpen = false;
                toolTip.SetOwner(null);
                ElementsAndToolTips.Remove(owner);
            }
        }

        private static void RegisterTooltip(UIElement owner, object p)
        {
            if (owner == null)
            {
                return;
            }

            var toolTip = p as PinnableTooltip ?? ConvertToTooltip(p);

            var element = owner as FrameworkElement;
            if (element != null)
            {
                element.Unloaded += FrameworkElementUnloaded;
                toolTip.DataContext = element.DataContext;
            }

            toolTip.SetOwner(owner);
            owner.MouseEnter += OnElementMouseEnter;
            owner.MouseLeave += OnElementMouseLeave;

            var control = owner as Control;
            if (control != null)
            {
                control.IsEnabledChanged += OnControlEnabledChanged;
            }

            lock (Locker)
            {
                ElementsAndToolTips[owner] = toolTip;
            }
        }

        private static void OnControlEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                return;
            }

            var element = (UIElement)sender;
            PinnableTooltip tooltip = null;
            lock (Locker)
            {
                if (ElementsAndToolTips.ContainsKey(element))
                {
                    tooltip = ElementsAndToolTips[element];
                }
            }

            if (tooltip != null)
            {
                tooltip.StopTimer();       
            }
        }

        private static void OnElementMouseLeave(object sender, MouseEventArgs e)
        {
            PinnableTooltip toolTip = null;

            lock (Locker)
            {
                var currentElement = (UIElement)sender;
                if (ElementsAndToolTips.ContainsKey(currentElement))
                {
                    toolTip = ElementsAndToolTips[currentElement];
                }
                else
                {
                    return;
                }
            }

            if (!toolTip.IsOpen)
            {
                toolTip.StopTimer();
            }
        }

        private static void OnElementMouseEnter(object sender, MouseEventArgs e)
        {
            UIElement currentElement;
            PinnableTooltip toolTip = null;

            lock (Locker)
            {
                currentElement = sender as UIElement;
                if (ElementsAndToolTips.ContainsKey(currentElement))
                {
                    toolTip = ElementsAndToolTips[currentElement];
                }
                else
                {
                    return;
                }

                MousePosition = e.GetPosition(null);
                SetRootVisual();
            }

            if (toolTip.Content == null 
                || toolTip.IsTimerEnabled || toolTip.IsOpen)
            {
                return;
            }

            var initialShowDelay = GetInitialShowDelay(currentElement);
            var showDuration = GetShowDuration(currentElement);

            if (initialShowDelay == 0)
            {
                toolTip.IsOpen = true;
            }

            toolTip.SetupTimer(initialShowDelay, showDuration);
            toolTip.StartTimer();
        }

        private static void SetRootVisual()
        {
            lock (Locker)
            {
                if ((RootVisual != null) || (Application.Current == null))
                {
                    return;
                }

#if (SILVERLIGHT)
                RootVisual = Application.Current.RootVisual as FrameworkElement;
#else
                RootVisual = BrowserInteropHelper.IsBrowserHosted ? null : Application.Current.MainWindow;
#endif
                if (RootVisual == null)
                {
                    return;
                }

                RootVisual.MouseMove += OnRootVisualMouseMove;
                RootVisual.AddHandler(
                    UIElement.MouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(OnRootVisualMouseLeftButtonDown),
                    true);
            }
        }

        private static void OnRootVisualMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lock (Locker)
            {
                foreach (var toolTip in ElementsAndToolTips.Values.Where(toolTip => !toolTip.IsPinned))
                {
#if (!SILVERLIGHT)
                    if (ScreenUtils.IsParentOf(toolTip, e.OriginalSource as DependencyObject))
                    {
                        continue;
                    }
#endif
                    toolTip.IsOpen = false;
                }
            }
        }

        private static void OnRootVisualMouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(null);
        }

        private static void FrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            UnregisterTooltip(sender as UIElement);
        }

        private static PinnableTooltip ConvertToTooltip(object p)
        {
            return p as PinnableTooltip ?? new PinnableTooltip { Content = p };
        }
    }
}
