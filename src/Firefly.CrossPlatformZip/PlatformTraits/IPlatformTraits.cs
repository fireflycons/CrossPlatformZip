// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlatformTraits.cs" company="">
//   
// </copyright>
// <summary>
//   Describes mechanism for generating ZIP external attributes field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Firefly.CrossPlatformZip.PlatformTraits
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///     Describes mechanism for generating ZIP external attributes field.
    /// </summary>
    internal interface IPlatformTraits
    {
        /// <summary>
        ///     Gets the platform-specific directory separator.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        char DirectorySeparator { get; }

        /// <summary>
        ///     Gets the directory separator for foreign OS, e.g Windows separator on POSIX and vice-versa.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        char ForeignDirectorySeparator { get; }

        /// <summary>
        /// Gets the host system identifier.
        /// </summary>
        /// <value>
        /// The host system identifier.
        /// </value>
        int HostSystemId { get; }

        /// <summary>
        /// Gets the ZIP external attributes for the given file system object.
        /// </summary>
        /// <param name="fileSystemObject">
        /// The file system object.
        /// </param>
        /// <returns>
        /// ZIP external attribute
        /// </returns>
        int GetExternalAttributes(FileSystemInfo fileSystemObject);

        /// <summary>
        /// Gets the extra data records (if any) to add to new entry.
        /// </summary>
        /// <param name="fileSystemObject">The file system object.</param>
        /// <returns>Byte array of raw extra data.</returns>
        byte[] GetExtraDataRecords(FileSystemInfo fileSystemObject);

        /// <summary>
        /// Pre-validates a list of items to be zipped.
        /// Throws if we should not continue.
        /// </summary>
        /// <param name="fileList">The file list.</param>
        void PreValidateFileList(IList<FileSystemInfo> fileList);
    }
}