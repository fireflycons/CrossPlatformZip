namespace Firefly.CrossPlatformZip.FileSystem
{
    internal class PosixAttributes
    {
        /// <summary>
        /// A catch all for any parse errors - all attributes set and owned by root
        /// </summary>
        public static readonly PosixAttributes AllForRoot =
            new PosixAttributes { Attributes = 0x1ff, Uid = 0, Gid = 0 };

        /// <summary>
        /// Gets or sets the file or directory attributes (e.g. <c>rwxr-xr-x</c>) as an integer bit field.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public int Attributes { get; set; }

        /// <summary>
        /// Gets or sets the group ID.
        /// </summary>
        /// <value>
        /// The group ID.
        /// </value>
        public int Gid { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        /// <value>
        /// The user ID.
        /// </value>
        public int Uid { get; set; }
    }
}