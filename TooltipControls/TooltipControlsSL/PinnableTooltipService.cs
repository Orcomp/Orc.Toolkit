// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PinnableTooltipService.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The pinnable tooltip service.
// </summary>
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
    using Orc.Toolkit.TooltipControls.Helpers;
#endif

    /// <summary>
    /// The pinnable tooltip service.
    /// </summary>
    public static class PinnableTooltipService
    {
        #region Constants

        /// <summary>
        /// The default initial show delay.
        /// </summary>
        private const int DefaultInitialShowDelay = 500;

        /// <summary>
        /// The default show duration.
        /// </summary>
        private const int DefaultShowDuration = 5000;

        #endregion

        #region Static Fields

        /// <summary>
        /// The initial show delay property.
        /// </summary>
        public static readonly DependencyProperty InitialShowDelayProperty =
            DependencyProperty.RegisterAttached(
                "InitialShowDelay", 
                typeof(int), 
                typeof(PinnableTooltipService), 
                new PropertyMetadata(DefaultInitialShowDelay));

        /// <summary>
        /// The placement property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached(
            "Placement", 
            typeof(PlacementMode), 
            typeof(PinnableTooltipService), 
            new PropertyMetadata(PlacementMode.Mouse, OnPlacementPropertyChanged));

        /// <summary>
        /// The placement target property.
        /// </summary>
        public static readonly DependencyProperty PlacementTargetProperty =
            DependencyProperty.RegisterAttached(
                "PlacementTarget", 
                typeof(UIElement), 
                typeof(PinnableTooltipService), 
                new PropertyMetadata(OnPlacementTargetPropertyChanged));

        /// <summary>
        /// The show duration property.
        /// </summary>
        public static readonly DependencyProperty ShowDurationProperty =
            DependencyProperty.RegisterAttached(
                "ShowDurartion", typeof(int), typeof(PinnableTooltipService), new PropertyMetadata(DefaultShowDuration));

        /// <summary>
        /// The tooltip property.
        /// </summary>
        public static readonly DependencyProperty TooltipProperty = DependencyProperty.RegisterAttached(
            "Tooltip", typeof(object), typeof(PinnableTooltipService), new PropertyMetadata(OnTooltipPropertyChanged));

        /// <summary>
        /// The elements and tool tips.
        /// </summary>
        private static readonly Dictionary<UIElement, PinnableTooltip> ElementsAndToolTips =
            new Dictionary<UIElement, PinnableTooltip>();

        /// <summary>
        /// The locker.
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// The mouse position.
        /// </summary>
        private static Point mousePosition;

        /// <summary>
        /// The root visual.
        /// </summary>
        private static FrameworkElement rootVisual;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
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

        /// <summary>
        /// Gets the root visual.
        /// </summary>
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get initial show delay.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetInitialShowDelay(DependencyObject element)
        {
            return (int)element.GetValue(InitialShowDelayProperty);
        }

        /// <summary>
        /// The get placement.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="PlacementMode"/>.
        /// </returns>
        public static PlacementMode GetPlacement(DependencyObject element)
        {
            return (PlacementMode)element.GetValue(PlacementProperty);
        }

        /// <summary>
        /// The get placement target.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
        public static UIElement GetPlacementTarget(DependencyObject element)
        {
            return (UIElement)element.GetValue(PlacementTargetProperty);
        }

        /// <summary>
        /// The get show duration.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetShowDuration(DependencyObject element)
        {
            return (int)element.GetValue(ShowDurationProperty);
        }

        /// <summary>
        /// The get tooltip.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object GetTooltip(DependencyObject element)
        {
            return element.GetValue(TooltipProperty);
        }

        /// <summary>
        /// The set initial show delay.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetInitialShowDelay(DependencyObject element, int value)
        {
            element.SetValue(InitialShowDelayProperty, value);
        }

        /// <summary>
        /// The set placement.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetPlacement(DependencyObject element, PlacementMode value)
        {
            element.SetValue(PlacementProperty, value);
        }

        /// <summary>
        /// The set placement target.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetPlacementTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(PlacementTargetProperty, value);
        }

        /// <summary>
        /// The set show duration.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetShowDuration(DependencyObject element, int value)
        {
            element.SetValue(ShowDurationProperty, value);
        }

        /// <summary>
        /// The set tooltip.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetTooltip(DependencyObject element, object value)
        {
            element.SetValue(TooltipProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert to tooltip.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="PinnableTooltip"/>.
        /// </returns>
        private static PinnableTooltip ConvertToTooltip(object p)
        {
            return p as PinnableTooltip ?? new PinnableTooltip { Content = p };
        }

        /// <summary>
        /// The framework element unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void FrameworkElementUnloaded(object sender, RoutedEventArgs e)
        {
            UnregisterTooltip(sender as UIElement);
        }

        /// <summary>
        /// The on control enabled changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The on element mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

            if (toolTip.Content == null || toolTip.IsTimerEnabled || toolTip.IsOpen)
            {
                return;
            }

            int initialShowDelay = GetInitialShowDelay(currentElement);
            int showDuration = GetShowDuration(currentElement);

            if (initialShowDelay == 0)
            {
                toolTip.IsOpen = true;
            }

            toolTip.SetupTimer(initialShowDelay, showDuration);
            toolTip.StartTimer();
        }

        /// <summary>
        /// The on element mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The on placement property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The on placement target property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The on root visual mouse left button down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnRootVisualMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lock (Locker)
            {
                foreach (PinnableTooltip toolTip in ElementsAndToolTips.Values.Where(toolTip => !toolTip.IsPinned))
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

        /// <summary>
        /// The on root visual mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void OnRootVisualMouseMove(object sender, MouseEventArgs e)
        {
            MousePosition = e.GetPosition(null);
        }

        /// <summary>
        /// The on tooltip property changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

        /// <summary>
        /// The register tooltip.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        private static void RegisterTooltip(UIElement owner, object p)
        {
            if (owner == null)
            {
                return;
            }

            PinnableTooltip toolTip = p as PinnableTooltip ?? ConvertToTooltip(p);

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

        /// <summary>
        /// The set root visual.
        /// </summary>
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

        /// <summary>
        /// The unregister tooltip.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
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

        #endregion
    }
}