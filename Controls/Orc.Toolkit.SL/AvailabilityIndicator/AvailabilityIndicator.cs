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
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The focus indicator button.
    /// </summary>
    [TemplateVisualState(Name = "NotAvailable", GroupName = "MainStates")]
    [TemplateVisualState(Name = "Available", GroupName = "MainStates")]
    public class AvailabilityIndicator : Control
    {
        /// <summary>
        /// The availability property.
        /// </summary>
        public static readonly DependencyProperty AvailabilityProperty = DependencyProperty.Register(
            "Availability",
            typeof(bool),
            typeof(AvailabilityIndicator),
            new PropertyMetadata(new PropertyChangedCallback(OnAvailabilityChanged)));

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailabilityIndicator"/> class.
        /// </summary>
        public AvailabilityIndicator()
        {
            this.DefaultStyleKey = typeof(AvailabilityIndicator);
        }

        /// <summary>
        /// Gets or sets a value indicating whether availability.
        /// </summary>
        public bool Availability
        {
            get
            {
                return (bool)this.GetValue(AvailabilityProperty);
            }

            set
            {
                this.SetValue(AvailabilityProperty, value);
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
        private static void OnAvailabilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AvailabilityIndicator availabilityIndicator = d as AvailabilityIndicator;
            bool newState = (bool)e.NewValue;

            VisualStateManager.GoToState(availabilityIndicator, newState ? "Available" : "Notavailable", false);
            var groups = VisualStateManager.GetVisualStateGroups(availabilityIndicator);
            System.Diagnostics.Debug.WriteLine(groups.Count);
        }
    }
}
