// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderPlusMinus.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The slider plus minus.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// The slider plus minus.
    /// </summary>
    [TemplatePart(Name = "PART_MinusButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_PlusButton", Type = typeof(RepeatButton))]
    public class SliderPlusMinus : Slider
    {
        #region Fields

        /// <summary>
        /// The minus button.
        /// </summary>
        private RepeatButton minusButton;

        /// <summary>
        /// The plus button.
        /// </summary>
        private RepeatButton plusButton;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderPlusMinus"/> class.
        /// </summary>
        public SliderPlusMinus()
        {
            this.DefaultStyleKey = typeof(SliderPlusMinus);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The on apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.minusButton = (RepeatButton)this.GetTemplateChild("PART_MinusButton");
            this.plusButton = (RepeatButton)this.GetTemplateChild("PART_PlusButton");

            if (this.minusButton != null)
            {
                this.minusButton.Click += this.minusButton_Click;
            }

            if (this.plusButton != null)
            {
                this.plusButton.Click += this.plusButton_Click;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The minus button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void minusButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value -= this.LargeChange;
        }

        /// <summary>
        /// The plus button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value += this.LargeChange;
        }

        #endregion
    }
}