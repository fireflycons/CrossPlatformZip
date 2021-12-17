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

    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    ///     Generate Windows attributes for file system object
    /// </summary>
    /// <seealso cref="IPlatformTraits" />
    internal class WindowsPlatformTraits : IPlatformTraits
    {
        /// <inheritdoc />
        public char DirectorySeparator => '\\';

        /// <inheritdoc />
        public char ForeignDirectorySeparator => '/';

        /// <inheritdoc />
        public int HostSystemId => (int)HostSystemID.WindowsNT;

        /// <inheritdoc />
        public int GetExternalAttributes(FileSystemInfo fileSystemObject)
        {
            // For now, if we are creating a zip targeting Windows from a Unix filesystem, just set file attribute to Archive.
            return fileSystemObject is DirectoryInfo
                       ? (int)FileAttributes.Directory
                       : Zipper.IsWindows
                           ? (int)File.GetAttributes(fileSystemObject.FullName)
                           : (int)FileAttributes.Archive;
        }

        /// <inheritdoc />
        public byte[] GetExtraDataRecords(FileSystemInfo fileSystemObject)
        {
            // Nothing for now
            return null;
        }

        /// <inheritdoc />
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