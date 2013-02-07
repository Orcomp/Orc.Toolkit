// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupPlacement.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The popup placement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.Toolkit
{
    using System.ComponentModel;

    /// <summary>
    /// The popup placement.
    /// </summary>
    [Browsable(true)]
    public enum PopupPlacement
    {
        /// <summary>
        /// The top.
        /// </summary>
        Top, 

        /// <summary>
        /// The right.
        /// </summary>
        Right, 

        /// <summary>
        /// The bottom.
        /// </summary>
        Bottom, 

        /// <summary>
        /// The left.
        /// </summary>
        Left
    }
}