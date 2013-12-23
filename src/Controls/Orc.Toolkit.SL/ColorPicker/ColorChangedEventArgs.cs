﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorChangedEventArgs.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The color changed event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System;
    using System.Windows.Media;

    /// <summary>
    /// The color changed event args.
    /// </summary>
    public class ColorChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newColor">
        /// The new color.
        /// </param>
        /// <param name="oldColor">
        /// The old color.
        /// </param>
        public ColorChangedEventArgs(Color newColor, Color oldColor)
        {
            this.NewColor = newColor;
            this.OldColor = oldColor;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the new color.
        /// </summary>
        public Color NewColor { get; set; }

        /// <summary>
        /// Gets or sets the old color.
        /// </summary>
        public Color OldColor { get; set; }

        #endregion
    }
}