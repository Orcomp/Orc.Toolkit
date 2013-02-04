// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PngGenerator.cs" company="ORC">
//   MS-PL
// </copyright>
// <summary>
//   The png generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.Toolkit.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;

    /// <summary>
    ///     The png generator.
    /// </summary>
    internal class PngGenerator
    {
        // PNG chunk header sequences.
        #region Static Fields

        /// <summary>
        ///     The png chunk idat.
        /// </summary>
        private static readonly byte[] PngChunkIdat = { 73, 68, 65, 84 };

        /// <summary>
        ///     The png chunk iend.
        /// </summary>
        private static readonly byte[] PngChunkIend = { 73, 69, 78, 68 };

        /// <summary>
        ///     The png chunk ihdr.
        /// </summary>
        private static readonly byte[] PngChunkIhdr = { 73, 72, 68, 82 };

        #endregion

        #region Fields

        /// <summary>
        ///     The height of the generated image in pixels.
        /// </summary>
        private readonly int m_imageHeight;

        /// <summary>
        ///     The width of the generated image in pixels.
        /// </summary>
        private readonly int m_imageWidth;

        /// <summary>
        ///     The image pixel data chunk.
        /// </summary>
        private PngChunk m_idatChunk;

        /// <summary>
        ///     The 'trailer' chunk.
        /// </summary>
        private PngChunk m_iendChunk;

        /// <summary>
        ///     The image header chunk.
        /// </summary>
        private PngChunk m_ihdrChunk;

        /// <summary>
        ///     Pixel data as it is represented in the PNG.
        /// </summary>
        /// <remarks>
        ///     There seem to be two limiting factors on performance with this
        ///     bitmap generator. One is how quickly Silverlight can load the image
        ///     we give it. (Pregenerating two images in memory, and flipping between
        ///     them on my test system manages the full frame rate for 1024x768, but
        ///     only 40fps at 1280x1024. Since the PNG streams are already right there
        ///     in memory, there's not a lot we can do about that.)
        ///     The other factor is how many copies we make of the data. The more copies
        ///     the slower it all goes. So the SetPixelColorData function writes pixels
        ///     directly in the format that'll be passed back to PNG to avoid an extra
        ///     copy. This is a bit freakish because we need to leave some holes to cope
        ///     with the way PNG wants the data formatted.
        ///     The biggest complication is that the data is nominally compressed using
        ///     the deflate algorithm. We cheat by using the 'uncompressible block'
        ///     option. This is designed to allow data that would get larger if you tried
        ///     to compress it to be packaged verbatim, but we're using it for everything
        ///     for simplicity - the 'compression' isn't compression at all here. This would
        ///     be dead simple except you can only store up to 0xffff bytes in a non-compressible
        ///     block. So we have to insert extra blocks regularly.
        ///     Also, each individual row of pixels is preceded by an extra 0, to indicate
        ///     that we're using the pass through PNG filter, again for simplicity.
        /// </remarks>
        private byte[] pngPixelData;

        #endregion

        // TODO: with or without alpha. (Maybe 2 Create methods?)
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PngGenerator"/> class.
        ///     Create a new PNG generator for an image of the specified size.
        /// </summary>
        /// <param name="width">
        /// Width of image in pixels.
        /// </param>
        /// <param name="height">
        /// Height of image in pixels.
        /// </param>
        public PngGenerator(int width, int height)
        {
            this.m_imageWidth = width;
            this.m_imageHeight = height;

            this.MakeImageHeaderChunk();
            this.MakeImageDataChunk();
            this.MakeImageEndChunk();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Create a new Stream object that returns the image in PNG format.
        /// </summary>
        /// <returns>
        ///     A stream containing PNG format data, representing the pixels
        ///     provided for this image.
        /// </returns>
        public Stream CreateStream()
        {
            PngChunk[] chunks = { this.m_ihdrChunk, this.m_idatChunk, this.m_iendChunk };
            return new PngStream(chunks);
        }

        /// <summary>
        /// Set the pixel data from which to generate this PNG as Color values.
        /// </summary>
        /// <param name="pixels">
        /// And array of Color values. This is assumed to contain
        ///     Width*Height pixels, where Width and Height are the values passed in at
        ///     construction.
        /// </param>
        public void SetPixelColorData(Color[] pixels)
        {
            // ADLER32 counters
            // This turns out to be relative expensive, which is a PITA, since it's pretty much
            // pointless in a PNG. This is something PNG inherits as a result of using the ZLIB
            // spec. In ZLIB it serves a purpose. But in PNG, all chunks have a CRC32, so the
            // data ends up being protected by both a CRC32 and an ADLER32. Last time I measured,
            // the CRC32 was taking about 20% of the CPU time, and 50% is spent in the loop
            // right here.
            // Tauntingly, if you just crop the pixel data chunk so it's too short to contain the
            // ADLER32, neither Silverlight nor WPF complain. (Not when I wrote this anyway.) And
            // it speeds things up a fair bit. But I don't want to depend on that.
            uint s1 = 1;
            uint s2 = 0;
            int adlerCount = 0;

            for (int row = 0; row < this.m_imageHeight; ++row)
            {
                int rowTargetOffset = row * (this.m_imageWidth * 3 + 1);
                int rowSourceOffset = row * this.m_imageWidth;
                double rowRatio = row / (double)this.m_imageHeight;

                // Each row starts with a 0, indicating the non-transforming filter -
                // this lets us write raw pixel data.
                // We don't write the value out - the image size is fixed, so these things
                // never move in the array, and they always keep their initial 0 value. But
                // We do need to accumulate it for the ADLER32 check
                s2 = s2 + s1; // No need to add to s1 - this is a zero byte value.
                adlerCount += 1;

                int pos = rowTargetOffset + 1;
                for (int column = 0; column < this.m_imageWidth; ++column)
                {
                    Color col = pixels[rowSourceOffset + column];

                    // Nasty duplicated code. But when I had it all nicely factored
                    // out, it was causing serious slowdowns (order of 20%), and we're
                    // inside the critical loop here.
                    // Note, the weird calculation was originally factored out into
                    // MapPixelDataOffset. Its job is to deal with the fact that even
                    // when we disable compression, we still have to insert a 5 byte
                    // deflate block header every 0xffff bytes.
                    byte v = col.R;
                    this.pngPixelData[pos + (pos / 0xffff) * 5 + 7] = v;
                    pos += 1;
                    s1 = s1 + v;
                    s2 = s2 + s1;

                    v = col.G;
                    this.pngPixelData[pos + (pos / 0xffff) * 5 + 7] = v;
                    pos += 1;
                    s1 = s1 + v;
                    s2 = s2 + s1;

                    v = col.B;
                    this.pngPixelData[pos + (pos / 0xffff) * 5 + 7] = v;
                    pos += 1;
                    s1 = s1 + v;
                    s2 = s2 + s1;

                    // Do we need to reduce  yet? 
                    adlerCount += 3;
                    if (adlerCount > 5546)
                    {
                        s1 %= 65521;
                        s2 %= 65521;
                        adlerCount = 0;
                    }
                }
            }

            s1 %= 65521;
            s2 %= 65521;
            uint adler32 = (s2 << 16) + s1;
            NetworkOrderBitConverter.GetBytes(adler32, this.pngPixelData, this.pngPixelData.Length - 4);
            this.m_idatChunk.SetData(this.pngPixelData);
        }

        #endregion

        // Not actually used now...Was useful while I unit
        // tested it, but turned out to be too slow to call as a function.
        // (It was used in the critical tight loop in SetPixelColorData, which is
        // where over half the CPU time goes when building a new image.)
        // Still called in a unit test...I'm reluctant to delete this even
        // though the code under test has been defactored due to (measured, real)
        // performance issues.
        #region Methods

        /// <summary>
        /// The map pixel data offset.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int MapPixelDataOffset(int offset)
        {
            return offset + (offset / 0xffff) * 5 // Add five byte deflate block header for every 0xffff bytes
                   + 7; // Initial deflate block header plus first block
        }

        /// <summary>
        ///     The make image data chunk.
        /// </summary>
        private void MakeImageDataChunk()
        {
            this.m_idatChunk = new PngChunk(PngChunkIdat);

            int preDeflateImageDataSize = (this.m_imageWidth * 3 + 1) * this.m_imageHeight;
            int pngDataSize = MapPixelDataOffset(preDeflateImageDataSize);
            this.pngPixelData = new byte[pngDataSize + 4]; // Extra 4 bytes for ADLER-32
            this.pngPixelData[0] = 0x78;
            this.pngPixelData[1] = 1;

            // Block headers are all 0 except for last one, which is 1.
            for (int blockStart = 0; blockStart < preDeflateImageDataSize - 0xffff; blockStart += 0xffff)
            {
                int blockOffset = MapPixelDataOffset(blockStart);
                this.pngPixelData[blockOffset - 4] = 0xff;
                this.pngPixelData[blockOffset - 3] = 0xff;

                // Next two bytes two's complement - already initialized to 0
            }

            int lastBlock = MapPixelDataOffset((preDeflateImageDataSize / 0xffff) * 0xffff);
            int lastBlockSize = preDeflateImageDataSize % 0xffff;
            this.pngPixelData[lastBlock - 5] = 1;
            this.pngPixelData[lastBlock - 4] = (byte)(lastBlockSize & 0xff);
            this.pngPixelData[lastBlock - 3] = (byte)((lastBlockSize >> 8) & 0xff);
            lastBlockSize = ~lastBlockSize;
            this.pngPixelData[lastBlock - 2] = (byte)(lastBlockSize & 0xff);
            this.pngPixelData[lastBlock - 1] = (byte)((lastBlockSize >> 8) & 0xff);
        }

        /// <summary>
        ///     The make image end chunk.
        /// </summary>
        private void MakeImageEndChunk()
        {
            this.m_iendChunk = new PngChunk(PngChunkIend);
        }

        /// <summary>
        ///     The make image header chunk.
        /// </summary>
        private void MakeImageHeaderChunk()
        {
            this.m_ihdrChunk = new PngChunk(PngChunkIhdr);
            var ihdrData = new byte[13];
            NetworkOrderBitConverter.GetBytes((uint)this.m_imageWidth, ihdrData, 0);
            NetworkOrderBitConverter.GetBytes((uint)this.m_imageHeight, ihdrData, 4);
            ihdrData[8] = 8; // Bits per channel
            ihdrData[9] = 2; // Colour model - RGB
            this.m_ihdrChunk.SetData(ihdrData);
        }

        #endregion

        /// <summary>
        ///     Stream that returns PNG data.
        /// </summary>
        /// <remarks>
        ///     We have a custom stream rather than just building the PNG in memory in
        ///     order to minimize copies. Copying pixels around is about the slowest
        ///     thing this library does, and it's slow enough to cause measurable
        ///     performance problems. The current version still has a couple of places
        ///     where we could reduce the number of copies further, but this class has
        ///     a particularly important role: it's the one Silverlight itself talks to.
        /// </remarks>
        private class PngStream : Stream
        {
            #region Static Fields

            /// <summary>
            ///     Signature that appears at the start of all PNG files.
            /// </summary>
            private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

            #endregion

            #region Fields

            /// <summary>
            ///     The chunks that make up this particular PNG stream.
            /// </summary>
            private readonly PngChunk[] m_chunks;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="PngStream"/> class.
            ///     Builds a PNG stream around a set of chunks.
            /// </summary>
            /// <param name="chunks">
            /// The chunks that make up this PNG image.
            /// </param>
            /// <remarks>
            /// See http://www.w3.org/TR/PNG/ for more detail on all
            ///     this stuff.
            /// </remarks>
            public PngStream(PngChunk[] chunks)
            {
                this.m_chunks = chunks;
            }

            #endregion

            // Miscellaneous dull stream members.
            #region Public Properties

            /// <summary>
            ///     Gets a value indicating whether can read.
            /// </summary>
            public override bool CanRead
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            ///     Gets a value indicating whether can seek.
            /// </summary>
            public override bool CanSeek
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            ///     Gets a value indicating whether can write.
            /// </summary>
            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///     Gets total length of stream.
            /// </summary>
            /// <remarks>
            ///     Silverlight calls this to find out how big a buffer to allocate
            ///     when loading the PNG.
            /// </remarks>
            public override long Length
            {
                get
                {
                    return this.m_chunks.Sum(c => c.Length) + 8;
                }
            }

            /// <summary>
            ///     Current offset in stream.
            /// </summary>
            public override long Position { get; set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            ///     The flush.
            /// </summary>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override void Flush()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Read bytes from the stream. (Called by Silverlight.)
            /// </summary>
            /// <param name="buffer">
            /// The buffer into which we put the data being read.
            /// </param>
            /// <param name="offset">
            /// Position in buffer at which to start copying.
            /// </param>
            /// <param name="count">
            /// How much to copy.
            /// </param>
            /// <returns>
            /// The number of bytes read.
            /// </returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                int written = 0;

                // We reset the chunk counts on entry to the Read.
                // This isn't hugely efficient in the event of multiple short
                // reads as it means we iterate through all the chunks we already
                // handled on each new read. However, both Silverlight and WPF
                // seem to ask us our Length up front and then allocate a buffer
                // large enough to do the whole thing in one read. So in practice
                // this is a non-issue on current versions. (And it seems unlikely
                // they'd chop a bitmap read into lots of tiny chunks.)
                int chunk = 0;
                int currentChunkStartOffset = 8; // Space for PNG signature
                byte[] currentChunkData = null;
                while (written < count)
                {
                    int amount = 1;
                    if (this.Position < 8)
                    {
                        amount = CopyUtilities.WriteAsMuchDataAsPossible(
                            buffer, offset + written, offset + count, PngSignature, (int)this.Position);
                    }
                    else
                    {
                        // Make sure the current chunk is the one that contains the current position.
                        while (chunk < this.m_chunks.Length)
                        {
                            if (currentChunkStartOffset + this.m_chunks[chunk].Length <= this.Position)
                            {
                                currentChunkStartOffset += this.m_chunks[chunk].Length;
                                chunk += 1;
                                currentChunkData = null;
                            }
                            else
                            {
                                // TODO: unnecessary copy of data. Would be much more efficient
                                // to read this directly into the caller's buffer. Need to modify
                                // chunk interface to offer a Seek. (Or make Read take a position
                                // - there's no real need to Read to be sequential.)
                                if (currentChunkData == null)
                                {
                                    currentChunkData = new byte[this.m_chunks[chunk].Length];
                                    this.m_chunks[chunk].Rewind();
                                    this.m_chunks[chunk].Read(currentChunkData, 0, currentChunkData.Length);
                                }

                                break;
                            }
                        }

                        if (chunk == this.m_chunks.Length)
                        {
                            return written;
                        }

                        amount = CopyUtilities.WriteAsMuchDataAsPossible(
                            buffer, 
                            offset + written, 
                            offset + count, 
                            currentChunkData, 
                            (int)this.Position - currentChunkStartOffset);
                    }

                    this.Position += amount;
                    written += amount;
                }

                return count;
            }

            /// <summary>
            /// Move to a specific location within the stream.
            /// </summary>
            /// <param name="offset">
            /// Distance to move.
            /// </param>
            /// <param name="origin">
            /// Point to which 'offset' argument is relative.
            /// </param>
            /// <returns>
            /// Current position.
            /// </returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        this.Position = offset;
                        break;

                    case SeekOrigin.Current:
                        this.Position += offset;
                        break;

                    case SeekOrigin.End:
                        this.Position = Math.Max(0, this.Length - offset);
                        break;
                }

                return this.Position;
            }

            // Methods not implemented because we're read-only.

            /// <summary>
            /// The set length.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// The write.
            /// </summary>
            /// <param name="buffer">
            /// The buffer.
            /// </param>
            /// <param name="offset">
            /// The offset.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}