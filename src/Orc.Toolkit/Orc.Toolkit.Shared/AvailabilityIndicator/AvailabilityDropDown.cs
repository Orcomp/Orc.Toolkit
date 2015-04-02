// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvailabilityDropDown.cs" company="ORC">
//   MS-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.Windows;

    /// <summary>
    /// The availability drop down.
    /// </summary>
    public class AvailabilityDropDown : DropDownButton
    {
        /// <summary>
        /// The availability property.
        /// </summary>
        public static readonly DependencyProperty IsAvailableProperty = DependencyProperty.Register(
            "IsAvailable",
            typeof(bool),
            typeof(AvailabilityDropDown),
            new PropertyMetadata(false));

        /// <summary>
        /// The is in use property.
        /// </summary>
        public static readonly DependencyProperty IsInUseProperty = DependencyProperty.Register(
            "IsInUse",
            typeof(bool),
            typeof(AvailabilityDropDown),
            new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailabilityDropDown"/> class.
        /// </summary>
        public AvailabilityDropDown()
        {
            this.DefaultStyleKey = typeof(AvailabilityDropDown);
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
                this.SetValue(IsInUseProperty, value);
            }
        }
    }
}
