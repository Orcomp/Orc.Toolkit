// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CopyUtilities.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The copy utilities.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit
{
    using System;

    /// <summary>
    ///     The copy utilities.
    /// </summary>
    internal class CopyUtilities
    {
        #region Public Methods and Operators

        /// <summary>
        /// Copy as much data as possible for a read operation.
        /// </summary>
        /// <param name="targetBuffer">
        /// Buffer to put data being read.
        /// </param>
        /// <param name="targetOffset">
        /// Position from which to start putting read data.
        /// </param>
        /// <param name="targetEnd">
        /// Offset at which no more data must be written.
        /// </param>
        /// <param name="source">
        /// Buffer from which data is being copied.
        /// </param>
        /// <param name="sourceOffset">
        /// Offset from which to start copying.
        /// </param>
        /// <returns>
        /// Number of bytes copied.
        /// </returns>
        /// <remarks>
        /// We frequently end up needing to copy data around, and the size of the source and
        ///     target buffers don't necessarily match up neatly. The simple approach is to copy
        ///     one byte at a time, but that has dreadful perf. This works out the largest amount
        ///     that can be copied while still fitting into the remaining space, and without
        ///     attempting to copy more data than there is.
        ///     The slightly peculiar bunch of arguments comes from the fact that this fell out
        ///     of a refactoring exercise. It's not a wonderfully useful general-purpose method.
        /// </remarks>
        public static int WriteAsMuchDataAsPossible(
            byte[] targetBuffer, int targetOffset, int targetEnd, byte[] source, int sourceOffset)
        {
            int spaceInTarget = targetEnd - targetOffset;
            int spaceInSource = source.Length - sourceOffset;
            int toCopy = Math.Min(spaceInSource, spaceInTarget);
            Array.Copy(source, sourceOffset, targetBuffer, targetOffset, toCopy);
            return toCopy;
        }

        #endregion
    }
}