// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PosixPlatformTraits.cs" company="">
//   
// </copyright>
// <summary>
//   Generate posix attributes for file system object
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Firefly.CrossPlatformZip.PlatformTraits
{
    using System.Collections.Generic;
    using System.IO;

    using Firefly.CrossPlatformZip.TaggedData;

    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    ///     Generate posix attributes for file system object
    /// </summary>
    /// <seealso cref="IPlatformTraits" />
    internal class PosixPlatformTraits : IPlatformTraits
    {
        /// <summary>
        ///     Gets the platform-specific directory separator.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        public char DirectorySeparator { get; } = '/';

        /// <summary>
        ///     Gets the directory separator for foreign OS, e.g Windows separator on POSIX and vice-versa.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        public char ForeignDirectorySeparator { get; } = '\\';

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
            // For now, rwxrwxrwx
            const int Attr = 0x1ff;

            return fileSystemObject is DirectoryInfo
                       ? ((Attr | 0x4000) << 16) | (int)FileAttributes.Directory
                       : (Attr | 0x8000) << 16;
        }

        /// <summary>
        /// Gets the extra data records (if any) to add to new entry.
        /// </summary>
        /// <param name="fileSystemObject">
        /// The file system object.
        /// </param>
        /// <returns>
        /// Byte array of raw extra data.
        /// </returns>
        public byte[] GetExtraDataRecords(FileSystemInfo fileSystemObject)
        {
            using (var zed = new ZipExtraData())
            {
                zed.AddEntry(
                    new ExtendedUnixData
                        {
                            ModificationTime =
                                fileSystemObject is DirectoryInfo
                                    ? Directory.GetLastWriteTimeUtc(fileSystemObject.FullName)
                                    : File.GetLastWriteTimeUtc(fileSystemObject.FullName),
                            AccessTime = fileSystemObject is DirectoryInfo
                                             ? Directory.GetLastAccessTime(fileSystemObject.FullName)
                                             : File.GetLastAccessTime(fileSystemObject.FullName)
                        });

                // root for now
                zed.AddEntry(new UnixExtraType3 { Uid = 0, Gid = 0 });
                return zed.GetEntryData();
            }
        }

        /// <summary>
        /// Pre-validates a list of items to be zipped.
        /// Currently nothing to do here.
        /// </summary>
        /// <param name="fileList">The file list.</param>
        public void PreValidateFileList(IList<FileSystemInfo> fileList)
        {
        }
    }
}