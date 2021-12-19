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
        public void PreValidateFileList(IList<FileSystemInfo> fileList)
        {
        }

        /// <inheritdoc />
        public PlatformData GetPlatformData(FileSystemInfo fileSystemObject)
        {
            int attr;
            var posixAttributes = PosixAttributes.AllForRoot;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (fileSystemObject is FileInfo fi)
                {
                    // We are running on Windows, but creating for Linux
                    // rwxrwxrwx | rw-rw-rw
                    attr = fi.IsExecutable() ? 0x1ff : 0x1b6;
                }
                else
                {
                    // Directory
                    attr = 0x1ff;
                }
            }
            else
            {
                // We are running on Linux = read fs permissions directly.
                posixAttributes = fileSystemObject.GetPosixAttributes();
                attr = posixAttributes.Attributes;
            }

            // Adjust to correct positions in the bit field, and ensure directories have x bit set
            attr = fileSystemObject is DirectoryInfo
                       ? ((attr | 0x4000) << 16) | (int)FileAttributes.Directory
                       : (attr | 0x8000) << 16;

            using (var zed = new ZipExtraData())
            {
                zed.AddEntry(
                    new ExtendedUnixData
                        {
                            CreateTime =
                                fileSystemObject is DirectoryInfo
                                    ? Directory.GetCreationTimeUtc(fileSystemObject.FullName)
                                    : File.GetCreationTimeUtc(fileSystemObject.FullName),
                            ModificationTime =
                                fileSystemObject is DirectoryInfo
                                    ? Directory.GetLastWriteTimeUtc(fileSystemObject.FullName)
                                    : File.GetLastWriteTimeUtc(fileSystemObject.FullName),
                            AccessTime = fileSystemObject is DirectoryInfo
                                             ? Directory.GetLastAccessTimeUtc(fileSystemObject.FullName)
                                             : File.GetLastAccessTimeUtc(fileSystemObject.FullName)
                        });

                zed.AddEntry(new UnixExtraType3 { Uid = posixAttributes.Uid, Gid = posixAttributes.Gid });
                return new PlatformData { Attributes = attr, ExtraData = zed.GetEntryData() };
            }
        }
    }
}