// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailabilityIndicator.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The focus indicator button.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The focus indicator button.
    /// </summary>
    [TemplateVisualState(Name = "NotAvailable", GroupName = "MainStates")]
    [TemplateVisualState(Name = "Available", GroupName = "MainStates")]
    [TemplateVisualState(Name = "IsInUse", GroupName = "MainStates")]
    public class AvailabilityIndicator : Control
    {
        /// <summary>
        /// The availability property.
        /// </summary>
        public static readonly DependencyProperty IsAvailableProperty = DependencyProperty.Register(
            "IsAvailable",
            typeof(bool),
            typeof(AvailabilityIndicator),
            new PropertyMetadata(false, new PropertyChangedCallback(OnStateChanged)));

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
        /// The in use started subscription.
        /// </summary>
        private IDisposable inUseStartedSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailabilityIndicator"/> class.
        /// </summary>
        public AvailabilityIndicator()
        {
            this.DefaultStyleKey = typeof(AvailabilityIndicator);
        }

        /// <summary>
        /// The is in use started.
        /// </summary>
        private event EventHandler IsInUseStarted;

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
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.SetVisualState();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// The on is in use started.
        /// </summary>
        protected virtual void OnIsInUseStarted()
        {
            EventHandler handler = this.IsInUseStarted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
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
            AvailabilityIndicator availabilityIndicator = d as AvailabilityIndicator;
            if (availabilityIndicator != null)
            {
                if (availabilityIndicator.inUseStartedSubscription != null)
                {
                    availabilityIndicator.inUseStartedSubscription.Dispose();
                }

                if (availabilityIndicator.InUsePeriod > 0)
                {
                    availabilityIndicator.inUseStartedSubscription =
                        Observable.FromEventPattern<EventHandler, EventArgs>(
                            h => availabilityIndicator.IsInUseStarted += h,
                            h => availabilityIndicator.IsInUseStarted -= h)
                                  .Throttle(TimeSpan.FromMilliseconds(500))
                                  .Delay(TimeSpan.FromMilliseconds(availabilityIndicator.InUsePeriod))
                                  .Subscribe(pattern => availabilityIndicator.IsInUse = false);
                }
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
            AvailabilityIndicator availabilityIndicator = d as AvailabilityIndicator;
            if (availabilityIndicator != null)
            {
                availabilityIndicator.SetVisualState();
                if (availabilityIndicator.IsInUse)
                {
                    availabilityIndicator.OnIsInUseStarted();
                }
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
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AvailabilityIndicator availabilityIndicator = d as AvailabilityIndicator;
            if (availabilityIndicator != null)
            {
                availabilityIndicator.SetVisualState();
            }
        }

        /// <summary>
        /// The set visual state.
        /// </summary>
        private void SetVisualState()
        {
            if (!this.IsAvailable)
            {
                VisualStateManager.GoToState(this, "NotAvailable", false);
            }
            else
            {
                VisualStateManager.GoToState(this, this.IsInUse ? "IsInUse" : "Available", false);
            }
        }
    }
}
