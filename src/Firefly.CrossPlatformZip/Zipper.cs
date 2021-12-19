namespace Firefly.CrossPlatformZip
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Firefly.CrossPlatformZip.FileSystem;
    using Firefly.CrossPlatformZip.PlatformTraits;
    using Firefly.CrossPlatformZip.TaggedData;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    ///     Cross platform zip utility
    /// </summary>
    public class Zipper
    {
        /// <summary>
        ///     What OS are we running on?
        /// </summary>
        internal static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Unzip a zip file, treating paths appropriately for the file system irrespective of path style in central directory.
        /// </summary>
        /// <param name="zipFile">
        /// The zip file.
        /// </param>
        /// <param name="directory">
        /// The directory.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Value cannot be null or empty - directory
        ///     or
        ///     Value cannot be null or empty - zipFile
        /// </exception>
        public static void Unzip(string zipFile, string directory)
        {
            if (!File.Exists(zipFile))
            {
                throw new FileNotFoundException("Cannot find zip file", zipFile);
            }

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(directory));
            }

            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(zipFile));
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var archive = new ZipInputStream(File.OpenRead(zipFile)))
            {
                const int BufSize = 4096;
                var buffer = new byte[BufSize];
                ZipEntry entry;

                while ((entry = archive.GetNextEntry()) != null)
                {
                    // Convert entry path to that expected by local file system.
                    var targetPath = Path.Combine(
                        directory,
                        IsWindows ? entry.Name.Replace('/', '\\') : entry.Name.Replace('\\', '/'));

                    if (entry.ExtraData != null && entry.ExtraData.Any())
                    {
                        var ed = new ZipExtraData(entry.ExtraData);

                        // TODO - Use this
                        var unixData = ed.GetData<ExtendedUnixData>();
                        var unixUserData = ed.GetData<UnixExtraType3>();
                    }

                    if (entry.IsDirectory)
                    {
                        if (Environment.UserInteractive)
                        {
                            Console.WriteLine($"Creating   {Path.GetFullPath(targetPath)}");
                        }

                        CreateDirectoryIfNotExists(targetPath);
                    }
                    else
                    {
                        // File entry - expand it.
                        CreateDirectoryIfNotExists(Path.GetDirectoryName(targetPath));

                        if (Environment.UserInteractive)
                        {
                            Console.WriteLine($"Extracting {Path.GetFullPath(targetPath)}");
                        }

                        using (var fs = File.OpenWrite(targetPath))
                        {
                            while (true)
                            {
                                var bytesRead = archive.Read(buffer, 0, BufSize);

                                if (bytesRead == 0)
                                {
                                    break;
                                }

                                fs.Write(buffer, 0, bytesRead);
                            }

                            fs.Flush();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Zips the specified zip file. Central directory is formatted for the operating system this method is called from,
        ///     i.e. if running in Linux, then paths and attributes are Linux in the zip.
        /// </summary>
        /// <param name="zipFile">
        /// The zip file.
        /// </param>
        /// <param name="path">
        /// The path to a file or folder to zip.
        /// </param>
        /// <param name="compressionLevel">Compression level (0 = store, 9 = best)</param>
        [Obsolete("Use Zip(CrossPlatformZipSettings)")]
        public static void Zip(string zipFile, string path, int compressionLevel)
        {
            Zip(
                new CrossPlatformZipSettings
                    {
                        Artifacts = path,
                        ZipFile = zipFile,
                        CompressionLevel = compressionLevel,
                        TargetPlatform = IsWindows ? ZipPlatform.Windows : ZipPlatform.Unix
                });
        }

        /// <summary>
        /// Zips the specified zip file, storing paths in the central directory appropriate for the target operating system.
        /// </summary>
        /// <param name="zipFile">
        /// The zip file.
        /// </param>
        /// <param name="path">
        /// The path to a file or folder to zip.
        /// </param>
        /// <param name="compressionLevel">Compression level (0 = store, 9 = best)</param>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Value cannot be null or empty - path
        ///     or
        ///     Value cannot be null or empty - zipFile
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// No files found to zip at '{path}
        /// </exception>
        [Obsolete("Use Zip(CrossPlatformZipSettings)")]
        public static void Zip(string zipFile, string path, int compressionLevel, ZipPlatform targetPlatform)
        {
            Zip(new CrossPlatformZipSettings
                    {
                        Artifacts = path,
                        ZipFile = zipFile,
                        CompressionLevel = compressionLevel,
                        TargetPlatform = targetPlatform
            });
        }

        /// <summary>
        /// Zips the specified zip file, storing paths in the central directory appropriate for the target operating system.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentException">
        /// 'Artifacts' cannot be null or empty - settings
        /// or
        /// 'ZipFile' cannot be null or empty - settings
        /// </exception>
        /// <exception cref="FileNotFoundException">No files found to zip at '{path}'</exception>
        public static void Zip(CrossPlatformZipSettings settings)
        {
            List<FileSystemInfo> filesToZip;
            DirectoryInfo zipRoot;

            if (string.IsNullOrWhiteSpace(settings.Artifacts))
            {
                throw new ArgumentException("'Artifacts' cannot be null or empty", nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.ZipFile))
            {
                throw new ArgumentException("'ZipFile' cannot be null or empty", nameof(settings));
            }

            var path = Path.GetFullPath(settings.Artifacts.Trim());

            if (Directory.Exists(path))
            {
                filesToZip = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories).Select(
                        f => File.Exists(f) ? new FileInfo(f) : (FileSystemInfo)new DirectoryInfo(f))
                    .ToList();

                zipRoot = new DirectoryInfo(path);
            }
            else if (!string.IsNullOrEmpty(settings.AlternateFileName))
            {
                ZipSingleFile(settings);
                return;
            }
            else
            {
                filesToZip = new List<FileSystemInfo> { new FileInfo(path) };

                // ReSharper disable once AssignNullToNotNullAttribute - already checked path is valid above
                zipRoot = new DirectoryInfo(Path.GetDirectoryName(path));
            }

            if (!filesToZip.Any())
            {
                throw new FileNotFoundException($"No files found to zip at '{path}'");
            }

            var platformTraits = PlatformTraitsGeneratorFactory.GetPlatformTraits(settings.TargetPlatform);

            platformTraits.PreValidateFileList(filesToZip);

            using (var archive = CreateZipFile(settings.ZipFile, settings.CompressionLevel, settings.TargetPlatform))
            {
                foreach (var fso in filesToZip)
                {
                    AddSingleEntry(archive, fso, zipRoot, platformTraits, settings.LogMessage, settings.LogError);
                }
            }
        }

        /// <summary>
        /// Zips a single file with optionally an alternate filename in the central directory entry.
        ///     Useful for e.g. creating a NodeJS AWS lambda package from a web packed project where the entry should be called
        ///     <c>index.js</c>
        /// </summary>
        /// <param name="zipFile">
        /// The zip file.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="alternateName">
        /// Name of entry to create in zip directory, or if <c>null</c>, use the original file name.
        /// </param>
        /// <param name="compressionLevel">Compression level (0 = store, 9 = best)</param>
        [Obsolete("Use ZipSingleFile(CrossPlatformZipSettings")]
        public static void ZipSingleFile(string zipFile, string filePath, string alternateName, int compressionLevel)
        {
            ZipSingleFile(
                new CrossPlatformZipSettings
                    {
                        Artifacts = filePath,
                        ZipFile = zipFile,
                        AlternateFileName = alternateName,
                        CompressionLevel = compressionLevel,
                        TargetPlatform = IsWindows ? ZipPlatform.Windows : ZipPlatform.Unix
                    });
        }

        /// <summary>
        /// Zips a single file with optionally an alternate filename in the central directory entry.
        ///     Useful for e.g. creating a Node JS AWS lambda package from a web packed project where the entry should be called
        ///     <c>index.js</c>
        /// </summary>
        /// <param name="zipFile">
        /// The zip file.
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="alternateName">
        /// Name of entry to create in zip directory, or if <c>null</c>, use the original file name.
        /// </param>
        /// <param name="compressionLevel">Compression level (0 = store, 9 = best)</param>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Value cannot be null or empty - filePath
        ///     or
        ///     Value cannot be null or empty - zipFile
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// File not found
        /// </exception>
        [Obsolete("Use ZipSingleFile(CrossPlatformZipSettings")]
        public static void ZipSingleFile(
            string zipFile,
            string filePath,
            string alternateName,
            int compressionLevel,
            ZipPlatform targetPlatform)
        {
            ZipSingleFile(
                new CrossPlatformZipSettings
                    {
                        Artifacts = filePath,
                        ZipFile = zipFile,
                        AlternateFileName = alternateName,
                        CompressionLevel = compressionLevel,
                        TargetPlatform = targetPlatform,
                        LogMessage = Console.WriteLine,
                        LogError = Console.Error.WriteLine
                    });
        }

        /// <summary>
        /// Zips a single file with optionally an alternate filename in the central directory entry.
        ///     Useful for e.g. creating a Node JS AWS lambda package from a web packed project where the entry should be called
        ///     <c>index.js</c>
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentException">
        /// 'Artifacts' cannot be null or empty - settings
        /// or
        /// 'ZipFile' cannot be null or empty - settings
        /// </exception>
        /// <exception cref="FileNotFoundException">File not found</exception>
        public static void ZipSingleFile(CrossPlatformZipSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Artifacts))
            {
                throw new ArgumentException("'Artifacts' cannot be null or empty", nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.ZipFile))
            {
                throw new ArgumentException("'ZipFile' cannot be null or empty", nameof(settings));
            }

            if (!File.Exists(settings.Artifacts))
            {
                throw new FileNotFoundException("File not found", settings.Artifacts);
            }

            var platformTraits = PlatformTraitsGeneratorFactory.GetPlatformTraits(settings.TargetPlatform);

            using (var archive = CreateZipFile(settings.ZipFile, settings.CompressionLevel, settings.TargetPlatform))
            {
                AddSingleEntry(
                    archive,
                    new FileInfo(settings.Artifacts),
                    // ReSharper disable once AssignNullToNotNullAttribute - should already have been verified
                    new DirectoryInfo(Path.GetDirectoryName(settings.Artifacts)),
                    platformTraits,
                    settings.LogMessage,
                    settings.LogError,
                    settings.AlternateFileName);
            }
        }

        /// <summary>Adds an entry to the zip archive.</summary>
        /// <param name="archive">The archive.</param>
        /// <param name="itemToAdd">The item to add.</param>
        /// <param name="zipRoot">The zip root.</param>
        /// <param name="platformTraits">The target platform.</param>
        /// <param name="logMessage">Sink for messages</param>
        /// <param name="logError">Sink for errors</param>
        /// <param name="alternateEntryName">Name of entry to create in zip directory, or if <c>null</c>, use the original file name.</param>
        private static void AddSingleEntry(
            ZipOutputStream archive,
            FileSystemInfo itemToAdd,
            DirectoryInfo zipRoot,
            IPlatformTraits platformTraits,
            Action<string> logMessage,
            Action<string> logError,
            string alternateEntryName = null)
        {
            var isDirectory = itemToAdd is DirectoryInfo;

            // Compute relative path
            var relative = itemToAdd.GetRelativePathFrom(zipRoot);

            if (!isDirectory && alternateEntryName != null)
            {
                var dir = Path.GetDirectoryName(relative);

                relative = !string.IsNullOrEmpty(dir) ? Path.Combine(dir, alternateEntryName) : alternateEntryName;
            }

            // Compute path for central directory
            var zipPath = relative.Replace(platformTraits.ForeignDirectorySeparator, platformTraits.DirectorySeparator).TrimEnd('\\', '/');

            if (isDirectory)
            {
                zipPath += platformTraits.DirectorySeparator;
            }

            if (Environment.UserInteractive)
            {
                logMessage($"Adding {zipPath}");
            }

            var platformData = platformTraits.GetPlatformData(itemToAdd);

            var entry = new ZipEntry(zipPath)
                            {
                                ExternalFileAttributes = platformData.Attributes,
                                ExtraData = platformData.ExtraData,
                                HostSystem = platformTraits.HostSystemId
                            };

            archive.PutNextEntry(entry);

            if (!entry.IsFile)
            {
                return;
            }

            var buf = new byte[4096];

            // Add the file
            using (var fs = File.OpenRead(itemToAdd.FullName))
            {
                StreamUtils.Copy(fs, archive, buf);
            }
        }

        /// <summary>
        /// Creates a directory if it doesn't exist.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        private static void CreateDirectoryIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Creates the zip file.
        /// </summary>
        /// <param name="zipFile">
        /// The zip file.
        /// </param>
        /// <param name="compressionLevel">Compression level (0 = store, 9 = best)</param>
        /// <param name="targetPlatform">
        /// The target Platform.
        /// </param>
        /// <returns>
        /// Open <see cref="ZipOutputStream"/>
        /// </returns>
        private static ZipOutputStream CreateZipFile(string zipFile, int compressionLevel, ZipPlatform targetPlatform)
        {
            if (compressionLevel < 0 || compressionLevel > 9)
            {
                throw new ArgumentException("Compression level must be between 0 and 9", nameof(compressionLevel));
            }

            if (Environment.UserInteractive)
            {
                Console.WriteLine(
                    $"Creating zip file '{zipFile}' with target platform {targetPlatform} and compression level {compressionLevel} (9 = best)");
            }

            var stream = new ZipOutputStream(File.Create(zipFile));

            stream.SetLevel(compressionLevel);
            return stream;
        }
    }
}