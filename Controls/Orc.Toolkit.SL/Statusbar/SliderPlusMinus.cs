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
    ///     The slider plus minus.
    /// </summary>
    [TemplatePart(Name = "PART_MinusButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_PlusButton", Type = typeof(RepeatButton))]
    public class SliderPlusMinus : Slider
    {
        ///// <summary>
        ///// The minimum bug fix property.
        ///// </summary>
        //public static readonly DependencyProperty MinimumBugFixProperty = DependencyProperty.Register("MinimumBugFix", typeof(double), typeof(SliderPlusMinus), new PropertyMetadata(1, OnUpdateSliderRange));
        public static readonly DependencyProperty MinimumBugFixProperty = DependencyProperty.RegisterAttached(
            "MinimumBugFix", typeof(double), typeof(SliderPlusMinus), new PropertyMetadata(OnUpdateSliderRange));
        

        ///// <summary>
        ///// The maximum bug fix property.
        ///// </summary>
        //public static readonly DependencyProperty MaximumBugFixProperty = DependencyProperty.Register("MaximumBugFix", typeof(double), typeof(SliderPlusMinus), new PropertyMetadata(10, OnUpdateSliderRange));
        public static readonly DependencyProperty MaximumBugFixProperty = DependencyProperty.RegisterAttached(
            "MaximumBugFix", typeof(double), typeof(SliderPlusMinus), new PropertyMetadata(OnUpdateSliderRange));
 
        #region Fields

        /// <summary>
        ///     The minus button.
        /// </summary>
        private RepeatButton minusButton;

        /// <summary>
        ///     The plus button.
        /// </summary>
        private RepeatButton plusButton;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SliderPlusMinus" /> class.
        /// </summary>
        public SliderPlusMinus()
        {
            this.DefaultStyleKey = typeof(SliderPlusMinus);
        }

        #endregion

        public double MinimumBugFix
        {
            get { return (double)GetValue(MinimumBugFixProperty); }
            set { SetValue(MinimumBugFixProperty, value); }
        }

        public double MaximumBugFix
        {
            get { return (double)GetValue(MaximumBugFixProperty); }
            set { SetValue(MaximumBugFixProperty, value); }
        }

        #region Public Methods and Operators

        /// <summary>
        ///     The on apply template.
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

        private static void OnUpdateSliderRange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SliderPlusMinus slider = d as SliderPlusMinus;
            if (slider != null)
            {
                if (slider.Maximum != slider.MaximumBugFix)
                {
                    slider.Maximum = slider.MaximumBugFix;
                }

                if (slider.Minimum != slider.MinimumBugFix)
                {
                    slider.Minimum = slider.MinimumBugFix;
                }
            }
        }

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