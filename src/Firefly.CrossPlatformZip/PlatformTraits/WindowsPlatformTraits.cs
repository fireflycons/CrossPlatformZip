// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsPlatformTraits.cs" company="">
//   
// </copyright>
// <summary>
//   Generate Windows attributes for file system object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Firefly.CrossPlatformZip.PlatformTraits
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Generate Windows attributes for file system object
    /// </summary>
    /// <seealso cref="IPlatformTraits" />
    internal class WindowsPlatformTraits : IPlatformTraits
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

        /// <summary>
        /// Pre-validates a list of items to be zipped.
        /// Throws if we should not continue.
        /// </summary>
        /// <param name="fileList">The file list.</param>
        /// <remarks>
        /// When creating a zip for Windows from Linux, we need to check that there are no case sensitivity issues with the input file list.
        /// </remarks>
        public void PreValidateFileList(IList<FileSystemInfo> fileList)
        {
            // Check files
            var duplicateFiles = fileList.Where(
                    f => fileList.Where(f1 => f1 is FileInfo).GroupBy(f1 => f1.FullName.ToLowerInvariant())
                        .Where(g => g.Count() > 1).Select(g => g.Key).Any(
                            d => string.Compare(d, f.FullName, StringComparison.OrdinalIgnoreCase) == 0))
                .Select(f => f.FullName)
                .ToList();

            var duplicateDirectories = fileList.Where(
                    f => fileList.Where(d => d is DirectoryInfo).GroupBy(d => d.FullName.ToLowerInvariant())
                        .Where(g => g.Count() > 1).Select(g => g.Key).Any(
                            d => string.Compare(d, f.FullName, StringComparison.OrdinalIgnoreCase) == 0))
                .Select(f => f.FullName)
                .ToList();

            if (!(duplicateDirectories.Any() || duplicateFiles.Any()))
            {
                return;
            }

            throw new DuplicateEntryException(
                      "Duplicate files and/or directories found where names differ only in case. This will not unzip correctly on Windows.")
                      {
                          DuplicateFiles
                              = duplicateFiles,
                          DuplicateDirectories
                              = duplicateDirectories
                      };
        }
    }
}