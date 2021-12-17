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
    using System.Runtime.InteropServices;

    using Firefly.CrossPlatformZip.FileSystem;
    using Firefly.CrossPlatformZip.TaggedData;

    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    ///     Generate POSIX attributes for file system object
    /// </summary>
    /// <seealso cref="IPlatformTraits" />
    internal class PosixPlatformTraits : IPlatformTraits
    {
        /// <inheritdoc />
        public char DirectorySeparator => '/';

        /// <inheritdoc />
        public char ForeignDirectorySeparator => '\\';

        /// <inheritdoc />
        public int HostSystemId => (int)HostSystemID.Unix;

        /// <inheritdoc />
        public int GetExternalAttributes(FileSystemInfo fileSystemObject)
        {
            int attr;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // We are running on Windows, but creating for Linux
                var isExecutable = false;

                if (fileSystemObject is FileInfo fi)
                {
                    isExecutable = fi.IsExecutable();
                }

                // rwxrwxrwx | rw-rw-rw
                attr = isExecutable ? 0x1ff : 0x1b6;
            }
            else
            {
                // We are running on Linux = read fs permissions directly.
                attr = fileSystemObject.GetPosixAttributes().Attributes;
            }

            return fileSystemObject is DirectoryInfo
                       ? ((attr | 0x4000) << 16) | (int)FileAttributes.Directory
                       : (attr | 0x8000) << 16;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void PreValidateFileList(IList<FileSystemInfo> fileList)
        {
        }
    }
}