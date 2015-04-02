// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTreeEnumeration.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Defines the VisualTreeEnumeration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Helpers
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// The visual tree enumeration.
    /// </summary>
    public static class VisualTreeEnumeration
    {
        /// <summary>
        /// The descendents.
        /// </summary>
        /// <param name="root">
        /// The root.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in Descendents(child))
                {
                    yield return descendent;
                }
            }
        }
    }
}
