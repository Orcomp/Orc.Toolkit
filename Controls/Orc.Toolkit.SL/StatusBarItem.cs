// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusBarItem.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The status bar item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The status bar item.
    /// </summary>
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    public class StatusBarItem : ContentControl
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBarItem"/> class.
        /// </summary>
        public StatusBarItem()
        {
            this.DefaultStyleKey = typeof(StatusBarItem);

            this.IsEnabledChanged += this.StatusBarItem_IsEnabledChanged;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", true);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The status bar item_ is enabled changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void StatusBarItem_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", true);
            }
        }

        #endregion
    }
}