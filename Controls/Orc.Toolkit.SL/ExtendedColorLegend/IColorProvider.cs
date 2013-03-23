// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IColorProvider.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   Represents an interface providing color item for color legend control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.ComponentModel;
    using System.Windows.Media;

    /// <summary>
    /// The ColorProvider interface.
    /// </summary>
    public interface IColorProvider : INotifyPropertyChanged
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets a value indicating whether color is visible or not
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        string Id { get; set; }

        #endregion
    }
}
