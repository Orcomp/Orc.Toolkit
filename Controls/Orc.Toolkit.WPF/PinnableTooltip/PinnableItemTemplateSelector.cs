// -----------------------------------------------------------------------
// <copyright file="PinnableItemTemplateSelector.cs" company="ORC">
// Selector for pinnable item template
// </copyright>
// -----------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The pinnable item template selector.
    /// </summary>
    public class PinnableItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The select template.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="DataTemplate"/>.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null)
            {
                if (item is OrdinalTooltipItem)
                {
                    return element.FindResource("OrdinalTooltipItemTemplate") as DataTemplate;
                }

                if (item is LinkTooltipItem)
                {
                    return element.FindResource("LinkTooltipItemTemplate") as DataTemplate;
                }
            }
            
            return null;
        }
    }
}
