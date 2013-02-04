// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Crc32.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The crc 32.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Helpers
{
    /// <summary>
    ///     The crc 32.
    /// </summary>
    internal static class Crc32
    {
        #region Static Fields

        /// <summary>
        ///     Table of CRCs of all 8-bit messages.
        /// </summary>
        private static readonly uint[] CrcTable = MakeCrcTable();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Calculates the CRC for the data provided.
        /// </summary>
        /// <param name="buffer">
        /// Data for which to calculate the CRC.
        /// </param>
        /// <returns>
        /// The CRC for this data.
        /// </returns>
        public static uint Crc(byte[] buffer)
        {
            return UpdateCrc(0xffffffff, buffer) ^ 0xffffffff;
        }

        /// <summary>
        /// Updates a running CRC.
        /// </summary>
        /// <param name="crc">
        /// Current CRC value. Use 0xffffffff if this is the first
        ///     run of bytes.
        /// </param>
        /// <param name="buffer">
        /// Bytes to be added into the current CRC.
        /// </param>
        /// <returns>
        /// The current CRC. Once done chaining CRC runs together,
        ///     XOR this with 0xffffffff to get the final output.
        /// </returns>
        public static uint UpdateCrc(uint crc, byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                crc = CrcTable[(crc ^ buffer[i]) & 0xff] ^ (crc >> 8);
            }

            return crc;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Precalculate a table for fast CRC generation.
        /// </summary>
        /// <returns>
        ///     The <see cref="uint[]" />.
        /// </returns>
        private static uint[] MakeCrcTable()
        {
            var table = new uint[256];

            for (int i = 0; i < table.Length; i++)
            {
                var current = (uint)i;
                for (int k = 0; k < 8; k++)
                {
                    if ((current & 1) != 0)
                    {
                        current = 0xedb88320 ^ (current >> 1);
                    }
                    else
                    {
                        current = current >> 1;
                    }
                }

                table[i] = current;
            }

            return table;
        }

        #endregion
    }
}