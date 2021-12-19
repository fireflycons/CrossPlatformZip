// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnixExtraType3.cs" company="">
//   
// </copyright>
// <summary>
//   The unix extra type 3.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Firefly.CrossPlatformZip.TaggedData
{
    using System.IO;

    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    /// Class that represents <c>ux: Unix Extra Type 3</c> as reported by <c>zipdetails</c> on Linux.
    /// This field stores Unix UID/GID data.
    /// </summary>
    internal class UnixExtraType3 : ITaggedData
    {
        /// <summary>
        /// The group ID size.
        /// </summary>
        private int gidSize = 4;

        /// <summary>
        /// The user ID size.
        /// </summary>
        private int uidSize = 4;

        /// <summary>
        /// The version.
        /// </summary>
        private int version = 1;

        /// <summary>
        /// Gets or sets the group ID.
        /// </summary>
        public int Gid { get; set; }

        /// <summary>
        /// Gets the tag id.
        /// </summary>
        public short TagID => 0x7875;

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Serializes this instance to byte array to write into zip directory.
        /// </summary>
        /// <returns>
        /// Raw byte array to write into zip directory entry
        /// </returns>
        /// <exception cref="ZipException">
        /// Unsupported integer size  for Unix UID or GID
        /// </exception>
        public byte[] GetData()
        {
            using (var ms = new MemoryStream())
            using (var helperStream = new TaggedDataHelperStream(ms))
            {
                helperStream.IsStreamOwner = false;

                helperStream.WriteByte((byte)this.version);
                helperStream.WriteByte((byte)this.uidSize);

                switch (this.uidSize)
                {
                    case 2:

                        helperStream.WriteLEShort(this.Uid);
                        break;

                    case 4:

                        helperStream.WriteLEInt(this.Uid);
                        break;

                    default:
                        throw new ZipException($"Unsupported integer size {this.uidSize} for Unix UID");
                }

                helperStream.WriteByte((byte)this.gidSize);

                switch (this.gidSize)
                {
                    case 2:

                        helperStream.WriteLEShort(this.Gid);
                        break;

                    case 4:

                        helperStream.WriteLEInt(this.Gid);
                        break;

                    default:
                        throw new ZipException($"Unsupported integer size {this.uidSize} for Unix GID");
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes raw data from zip directory entry into this object
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        public void SetData(byte[] data, int offset, int count)
        {
            using (var ms = new MemoryStream(data, offset, count, false))
            using (var helperStream = new TaggedDataHelperStream(ms))
            {
                this.version = helperStream.ReadByte();
                this.uidSize = helperStream.ReadByte();
                this.Uid = this.uidSize == 4 ? helperStream.ReadLEInt() : helperStream.ReadLEShort();
                this.gidSize = helperStream.ReadByte();
                this.Gid = this.gidSize == 4 ? helperStream.ReadLEInt() : helperStream.ReadLEShort();
            }
        }
    }
}