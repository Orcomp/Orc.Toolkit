// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkOrderBitConverter.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The network order bit converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Helpers
{
    /// <summary>
    ///     The network order bit converter.
    /// </summary>
    internal class NetworkOrderBitConverter
    {
        #region Methods

        /// <summary>
        /// Converts a UInt32 into its network order (big endian) byte representation.
        /// </summary>
        /// <param name="value">
        /// The 32-bit unsigned integer value to be converted.
        /// </param>
        /// <param name="buff">
        /// The buff.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        internal static void GetBytes(uint value, byte[] buff, int offset)
        {
            buff[offset] = (byte)((value >> 24) & 0xff);
            buff[offset + 1] = (byte)((value >> 16) & 0xff);
            buff[offset + 2] = (byte)((value >> 8) & 0xff);
            buff[offset + 3] = (byte)(value & 0xff);
        }

        /// <summary>
        /// Converts a UInt32 into its network order (big endian) byte representation.
        /// </summary>
        /// <param name="value">
        /// The 32-bit unsigned integer value to be converted.
        /// </param>
        /// <returns>
        /// A 4-byte array containing the big-endian representation of the value.
        /// </returns>
        internal static byte[] GetBytes(uint value)
        {
            var bytes = new byte[4];
            GetBytes(value, bytes, 0);
            return bytes;
        }

        /// <summary>
        /// Converts 4 bytes in network (big-endian) order into an UInt32.
        /// </summary>
        /// <param name="value">
        /// Array holding the bytes to be converted.
        /// </param>
        /// <param name="startIndex">
        /// Offset into array at which bytes are location.
        /// </param>
        /// <returns>
        /// The unsigned 32-bit value represented by the 4 bytes in the array.
        /// </returns>
        internal static uint ToUint32(byte[] value, int startIndex)
        {
            return
                (uint)
                ((value[startIndex] << 24) + (value[startIndex + 1] << 16) + (value[startIndex + 2] << 8)
                 + value[startIndex + 3]);
        }

        #endregion
    }
}