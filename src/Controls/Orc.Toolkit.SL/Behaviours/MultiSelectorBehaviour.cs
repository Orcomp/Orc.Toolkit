// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectorBehaviour.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Commands for ExtendedColorLegend control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Behaviours
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// A sync behaviour for a multiselector.
    /// </summary>
    public static class ListMultiSelectorBehaviour
    {
        /// <summary>
        /// The is pan enabled property.
        /// </summary>
        public static readonly DependencyProperty IsMultiSelectEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsMultiSelectEnabled",
                typeof(bool),
                typeof(ListMultiSelectorBehaviour),
                new PropertyMetadata(false, OnIsMultiSelectEnabledChanged));

        /// <summary>
        /// Property where selected items can be accessed
        /// </summary>
        public static readonly DependencyProperty UserSelectedItemsProperty = DependencyProperty.RegisterAttached(
            "UserSelectedItems", typeof(IList), typeof(ListMultiSelectorBehaviour), new PropertyMetadata(null));

        /// <summary>
        /// The get pan enabled.
        /// </summary>
        /// <param name="obj">
        /// The object
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetIsMultiSelectEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMultiSelectEnabledProperty);
        }

        /// <summary>
        /// Gets user selected items
        /// </summary>
        /// <param name="obj">dependency object</param>
        /// <returns>the result</returns>
        public static IList GetUserSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(UserSelectedItemsProperty);
        }

        /// <summary>
        /// The set pan enabled.
        /// </summary>
        /// <param name="obj">
        /// The object
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public static void SetIsMultiSelectEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMultiSelectEnabledProperty, value);
        }

        /// <summary>
        /// Sets selected items.
        /// </summary>
        /// <param name="obj">dependency object</param>
        /// <param name="value">the value</param>
        public static void SetUserSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(UserSelectedItemsProperty, value);
        }

        /// <summary>
        /// Initialization fo the list
        /// </summary>
        /// <param name="obj">Dependency object that must be ListBox</param>
        /// <param name="e">change parameters</param>
        private static void OnIsMultiSelectEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var listBox = obj as ListBox;
            if (listBox == null)
            {
                return;
            }

            if (e.NewValue is bool == false)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                listBox.SelectionChanged += (sender, args) => obj.SetValue(UserSelectedItemsProperty, listBox.SelectedItems);
            }
        }
    }
}
