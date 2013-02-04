// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PngChunk.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The png chunk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Helpers
{
    /// <summary>
    ///     The png chunk.
    /// </summary>
    internal class PngChunk
    {
        #region Fields

        /// <summary>
        ///     4-byte array containing the PNG chunk type.
        /// </summary>
        private readonly byte[] m_chunkType;

        /// <summary>
        ///     The CRC for this chunk.
        /// </summary>
        private byte[] m_crcBytes;

        /// <summary>
        ///     The data for this chunk, or null if there is no data.
        /// </summary>
        private byte[] m_data;

        /// <summary>
        ///     4-byte array containing the chunk length, in network order.
        /// </summary>
        private byte[] m_length;

        /// <summary>
        ///     The current position into the byte representation of this chunk for the
        ///     next Read operation.
        /// </summary>
        private int position;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PngChunk"/> class.
        ///     Construct a new PNG chunk of the specified chunk type.
        /// </summary>
        /// <param name="chunkType">
        /// 4 byte array containing the chunk type.
        /// </param>
        /// <remarks>
        /// TODO: Should we force passing of data (allowing null) here, and then change
        ///     SetData to UpdateData?
        /// </remarks>
        public PngChunk(byte[] chunkType)
        {
            this.m_chunkType = chunkType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the total length of chunk's byte representation.
        /// </summary>
        public int Length
        {
            get
            {
                return 12 + (this.m_data == null ? 0 : this.m_data.Length);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Read bytes from this chunk.
        /// </summary>
        /// <param name="buffer">
        /// Buffer into which to write bytes.
        /// </param>
        /// <param name="offset">
        /// Offset in buffer at which to start
        ///     writing bytes.
        /// </param>
        /// <param name="count">
        /// Maximum number of bytes to fetch.
        /// </param>
        /// <returns>
        /// The number of bytes placed into the buffer. If the
        ///     buffer is larger than the amount of remaining data, this will
        ///     be less than count. Returns 0 to indicate that no data is left.
        /// </returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            int dataLength = this.m_data == null ? 0 : this.m_data.Length;
            if (this.m_data == null)
            {
                this.m_length = new byte[4];
                this.m_crcBytes = NetworkOrderBitConverter.GetBytes(Crc32.Crc(this.m_chunkType));
            }

            int written = 0;
            while (written < count)
            {
                int amount = 1;
                if (this.position < 4)
                {
                    amount = CopyUtilities.WriteAsMuchDataAsPossible(
                        buffer, offset + written, offset + count, this.m_length, this.position);
                }
                else if (this.position < 8)
                {
                    amount = CopyUtilities.WriteAsMuchDataAsPossible(
                        buffer, offset + written, offset + count, this.m_chunkType, this.position - 4);
                }
                else if (this.position < (8 + dataLength))
                {
                    amount = CopyUtilities.WriteAsMuchDataAsPossible(
                        buffer, offset + written, offset + count, this.m_data, this.position - 8);
                }
                else if (this.position < (12 + dataLength))
                {
                    amount = CopyUtilities.WriteAsMuchDataAsPossible(
                        buffer, offset + written, offset + count, this.m_crcBytes, this.position - (8 + dataLength));
                }
                else
                {
                    return written;
                }

                this.position += amount;
                written += amount;
            }

            return count;
        }

        /// <summary>
        ///     Resets the Read position.
        /// </summary>
        /// <remarks>
        ///     Repeated calls to Read fetch data progressively from the chunk. Calling
        ///     this method resets the position back to the start, so the data can be
        ///     fetched again.
        /// </remarks>
        public void Rewind()
        {
            this.position = 0;
        }

        /// <summary>
        /// Provide the data for this chunk. (Not all chunks have data.)
        /// </summary>
        /// <param name="data">
        /// The data for this chunk.
        /// </param>
        public void SetData(byte[] data)
        {
            this.m_data = data;
            int length = this.m_data == null ? 0 : this.m_data.Length;
            this.m_length = NetworkOrderBitConverter.GetBytes((uint)length);
            uint crc = Crc32.UpdateCrc(0xffffffff, this.m_chunkType);
            if (this.m_data != null)
            {
                crc = Crc32.UpdateCrc(crc, this.m_data);
            }

            crc ^= 0xffffffff;
            this.m_crcBytes = NetworkOrderBitConverter.GetBytes(crc);
        }

        #endregion
    }
}