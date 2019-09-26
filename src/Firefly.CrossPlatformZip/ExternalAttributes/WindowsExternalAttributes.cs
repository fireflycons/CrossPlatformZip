﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsExternalAttributes.cs" company="">
//   
// </copyright>
// <summary>
//   Generate Windows attributes for file system object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Firefly.CrossPlatformZip.ExternalAttributes
{
    using System.IO;

    /// <summary>
    ///     Generate Windows attributes for file system object
    /// </summary>
    /// <seealso cref="Firefly.CrossPlatformZip.ExternalAttributes.IExternalAttributes" />
    internal class WindowsExternalAttributes : IExternalAttributes
    {
        /// <summary>
        ///     Gets the platform-specific directory separator.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        public char DirectorySeparator { get; } = '\\';

        /// <summary>
        ///     Gets the directory separator for foreign OS, e.g Windows separator on POSIX and vice-versa.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        public char ForeignDirectorySeparator { get; } = '/';

        /// <summary>
        /// Gets the ZIP external attributes for the given file system object.
        /// </summary>
        /// <param name="fileSystemObject">
        /// The file system object.
        /// </param>
        /// <returns>
        /// ZIP external attribute
        /// </returns>
        public int GetExternalAttributes(FileSystemInfo fileSystemObject)
        {
            // For now, if we are creating a zip targeting Windows from a Unix filesystem, just set file attribute to Archive.
            return fileSystemObject is DirectoryInfo ? (int)FileAttributes.Directory : (Zipper.IsWindows ? (int)File.GetAttributes(fileSystemObject.FullName) : (int)FileAttributes.Archive);
        }

        /// <summary>
        /// Gets the extra data records (if any) to add to new entry.
        /// </summary>
        /// <param name="fileSystemObject">The file system object.</param>
        /// <returns>
        /// Byte array of raw extra data.
        /// </returns>
        public byte[] GetExtraDataRecords(FileSystemInfo fileSystemObject)
        {
            // Nothing for now
            return null;
        }
    }
}