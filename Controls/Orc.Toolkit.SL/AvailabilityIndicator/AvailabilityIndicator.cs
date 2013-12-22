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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// The focus indicator button.
    /// </summary>
    [TemplateVisualState(Name = "NotAvailable", GroupName = "MainStates")]
    [TemplateVisualState(Name = "Available", GroupName = "MainStates")]
    [TemplateVisualState(Name = "IsInUse", GroupName = "MainStates")]
    public class AvailabilityIndicator : ToggleButton
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
        /// Initializes a new instance of the <see cref="AvailabilityIndicator"/> class.
        /// </summary>
        public AvailabilityIndicator()
        {
            this.DefaultStyleKey = typeof(AvailabilityIndicator);
        }

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
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.SetVisualState();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// The on mouse left button down.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (this.Focus())
            {
                System.Diagnostics.Debug.WriteLine("Sucessufully set focus");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Failed to setfocus");
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
