// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IControlAdornerChild.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The ControlAdornerChild interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Orc.Toolkit.Helpers
{
    using System.Windows;

    /// <summary>
    ///     The ControlAdornerChild interface.
    /// </summary>
    internal interface IControlAdornerChild
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get position.
        /// </summary>
        /// <returns>
        ///     The <see cref="Point" />.
        /// </returns>
        Point GetPosition();

        #endregion
    }
}

#endif