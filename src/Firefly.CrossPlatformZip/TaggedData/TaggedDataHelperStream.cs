// ReSharper disable InconsistentNaming
namespace Firefly.CrossPlatformZip.TaggedData
{
    using System.IO;

    /// <summary>
    ///     This class assists with writing/reading tagged extra field data.
    /// </summary>
    internal class TaggedDataHelperStream : Stream
    {
        #region Instance Fields

        /// <summary>
        /// The wrapped stream.
        /// </summary>
        private Stream stream;

        #endregion Instance Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaggedDataHelperStream"/> class. 
        /// Initialise a new instance of <see cref="TaggedDataHelperStream"/>.
        /// </summary>
        /// <param name="stream">
        /// The stream to use.
        /// </param>
        public TaggedDataHelperStream(Stream stream)
        {
            this.stream = stream;
        }

        #endregion Constructors

        /// <summary>
        ///     Gets or sets a value indicating whether the the underlying stream is owned or not.
        /// </summary>
        /// <remarks>If the stream is owned it is closed when this instance is closed.</remarks>
        public bool IsStreamOwner { get; set; }

        #region Base Stream Methods

        /// <summary>
        /// Gets a value indicating whether can read.
        /// </summary>
        public override bool CanRead => this.stream.CanRead;

        /// <summary>
        /// Gets a value indicating whether can seek.
        /// </summary>
        public override bool CanSeek => this.stream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether can timeout.
        /// </summary>
        public override bool CanTimeout => this.stream.CanTimeout;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public override long Length => this.stream.Length;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public override long Position
        {
            get => this.stream.Position;

            set => this.stream.Position = value;
        }

        /// <summary>
        /// Gets a value indicating whether can write.
        /// </summary>
        public override bool CanWrite => this.stream.CanWrite;

        /// <summary>
        /// The flush.
        /// </summary>
        public override void Flush()
        {
            this.stream.Flush();
        }

        /// <summary>
        /// The seek.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        /// <summary>
        /// The set length.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        /// <summary>
        /// The read.
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
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
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
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        #endregion Base Stream Methods

        #region LE value reading/writing

        /// <summary>
        ///     Read an unsigned short in little endian byte order.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        /// <exception cref="IOException">
        ///     An i/o error occurs.
        /// </exception>
        /// <exception cref="EndOfStreamException">
        ///     The file ends prematurely
        /// </exception>
        public int ReadLEShort()
        {
            int byteValue1 = this.stream.ReadByte();

            if (byteValue1 < 0)
            {
                throw new EndOfStreamException();
            }

            int byteValue2 = this.stream.ReadByte();
            if (byteValue2 < 0)
            {
                throw new EndOfStreamException();
            }

            return byteValue1 | (byteValue2 << 8);
        }

        /// <summary>
        ///     Read an int in little endian byte order.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        /// <exception cref="IOException">
        ///     An i/o error occurs.
        /// </exception>
        /// <exception cref="System.IO.EndOfStreamException">
        ///     The file ends prematurely
        /// </exception>
        public int ReadLEInt()
        {
            return this.ReadLEShort() | (this.ReadLEShort() << 16);
        }

        /// <summary>
        ///     Read a long in little endian byte order.
        /// </summary>
        /// <returns>The value read.</returns>
        public long ReadLELong()
        {
            return (uint)this.ReadLEInt() | ((long)this.ReadLEInt() << 32);
        }

        /// <summary>
        /// Write an unsigned short in little endian byte order.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        public void WriteLEShort(int value)
        {
            this.stream.WriteByte((byte)(value & 0xff));
            this.stream.WriteByte((byte)((value >> 8) & 0xff));
        }

        /// <summary>
        /// Write a ushort in little endian byte order.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        public void WriteLEUshort(ushort value)
        {
            this.stream.WriteByte((byte)(value & 0xff));
            this.stream.WriteByte((byte)(value >> 8));
        }

        /// <summary>
        /// Write an int in little endian byte order.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        public void WriteLEInt(int value)
        {
            this.WriteLEShort(value);
            this.WriteLEShort(value >> 16);
        }

        /// <summary>
        /// Write a uint in little endian byte order.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        public void WriteLEUint(uint value)
        {
            this.WriteLEUshort((ushort)(value & 0xffff));
            this.WriteLEUshort((ushort)(value >> 16));
        }

        /// <summary>
        /// Write a long in little endian byte order.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        public void WriteLELong(long value)
        {
            this.WriteLEInt((int)value);
            this.WriteLEInt((int)(value >> 32));
        }

        /// <summary>
        /// Write a ulong in little endian byte order.
        /// </summary>
        /// <param name="value">
        /// The value to write.
        /// </param>
        public void WriteLEUlong(ulong value)
        {
            this.WriteLEUint((uint)(value & 0xffffffff));
            this.WriteLEUint((uint)(value >> 32));
        }

        #endregion LE value reading/writing

        /// <summary>
        /// Close the stream.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        /// <remarks>
        /// The underlying stream is closed only if <see cref="IsStreamOwner"/> is true.
        /// </remarks>
        protected override void Dispose(bool disposing)
        {
            Stream toClose = this.stream;
            this.stream = null;
            if (this.IsStreamOwner && (toClose != null))
            {
                this.IsStreamOwner = false;
                toClose.Dispose();
            }
        }
    }
}