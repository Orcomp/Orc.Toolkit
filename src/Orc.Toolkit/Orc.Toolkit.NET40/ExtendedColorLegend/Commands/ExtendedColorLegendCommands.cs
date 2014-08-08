// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedColorLegendCommands.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Commands for ExtendedColorLegend control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit.Commands
{
    using System.Windows.Input;

    /// <summary>
    /// Commands for ExtendedColorLegend control.
    /// </summary>
    public class ExtendedColorLegendCommands
    {
        /// <summary>
        /// Clear filter command
        /// </summary>
        private static readonly RoutedCommand ClearFilterCommand = new RoutedCommand();

        /// <summary>
        /// Gets instance of clear filter command
        /// </summary>
        public static RoutedCommand ClearFilter
        {
            get
            {
                return ClearFilterCommand;
            }
        }
    }
}
