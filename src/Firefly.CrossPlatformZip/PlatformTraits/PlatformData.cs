using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.CrossPlatformZip.PlatformTraits
{
    /// <summary>
    /// Platform specific zip entry data
    /// </summary>
    internal class PlatformData
    {
        /// <summary>
        /// Gets or sets the file attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public int Attributes { get; set; }

        /// <summary>
        /// Gets or sets the extra data records.
        /// </summary>
        /// <value>
        /// The extra data.
        /// </value>
        public byte[] ExtraData { get; set; }
    }
}
