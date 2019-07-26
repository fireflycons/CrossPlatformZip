﻿namespace Firefly.CrossPlatformZip
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    /// <summary>
    ///     Cross platform zip utility
    /// </summary>
    public class Zipper
    {
        /// <summary>
        ///     What OS is the running on?
        /// </summary>
        private static readonly bool IsWindows = Path.DirectorySeparatorChar == '\\';

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
        public static void Zip(string zipFile, string path, int compressionLevel)
        {
            Zip(zipFile, path, compressionLevel, IsWindows ? ZipPlatform.Windows : ZipPlatform.Unix);
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
        public static void Zip(string zipFile, string path, int compressionLevel, ZipPlatform targetPlatform)
        {
            List<FileSystemInfo> filesToZip;
            DirectoryInfo zipRoot;
            var separator = targetPlatform == ZipPlatform.Windows ? '\\' : '/';

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(path));
            }

            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(zipFile));
            }

            path = Path.GetFullPath(path.Trim());

            if (Directory.Exists(path))
            {
                filesToZip = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.AllDirectories).Select(
                        f => (File.Exists(f) ? (FileSystemInfo)new FileInfo(f) : (FileSystemInfo)new DirectoryInfo(f)))
                    .ToList();

                zipRoot = new DirectoryInfo(path);
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

            using (var archive = CreateZipFile(zipFile, compressionLevel, targetPlatform))
            {
                foreach (var fso in filesToZip)
                {
                    AddSingleEntry(archive, fso, zipRoot, targetPlatform, separator);
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
        public static void ZipSingleFile(string zipFile, string filePath, string alternateName, int compressionLevel)
        {
            ZipSingleFile(
                zipFile,
                filePath,
                alternateName,
                compressionLevel,
                IsWindows ? ZipPlatform.Windows : ZipPlatform.Unix);
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
        public static void ZipSingleFile(
            string zipFile,
            string filePath,
            string alternateName,
            int compressionLevel,
            ZipPlatform targetPlatform)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(filePath));
            }

            if (string.IsNullOrWhiteSpace(zipFile))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(zipFile));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            using (var archive = CreateZipFile(zipFile, compressionLevel, targetPlatform))
            {
                AddSingleEntry(
                    archive,
                    new FileInfo(filePath),
                    // ReSharper disable once AssignNullToNotNullAttribute - should already have been verified
                    new DirectoryInfo(Path.GetDirectoryName(filePath)),
                    targetPlatform,
                    targetPlatform == ZipPlatform.Windows ? '\\' : '/',
                    alternateName);
            }
        }

        /// <summary>
        /// Adds an entry to the zip archive.
        /// </summary>
        /// <param name="archive">
        /// The archive.
        /// </param>
        /// <param name="itemToAdd">
        /// The item to add.
        /// </param>
        /// <param name="zipRoot">
        /// The zip root.
        /// </param>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        /// <param name="directorySeparator">
        /// The directory separator.
        /// </param>
        /// <param name="alternateEntryName">
        /// Name of entry to create in zip directory, or if <c>null</c>, use the original file
        ///     name.
        /// </param>
        private static void AddSingleEntry(
            ZipOutputStream archive,
            FileSystemInfo itemToAdd,
            DirectoryInfo zipRoot,
            ZipPlatform targetPlatform,
            char directorySeparator,
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
            var zipPath =
                (targetPlatform == ZipPlatform.Unix
                     ? relative.Replace('\\', directorySeparator)
                     : relative.Replace('/', '\\')).TrimEnd('\\', '/');

            if (isDirectory)
            {
                zipPath += directorySeparator;
            }

            if (Environment.UserInteractive)
            {
                Console.WriteLine($"Adding {zipPath}");
            }

            var entry = new ZipEntry(zipPath);

            if (targetPlatform == ZipPlatform.Unix)
            {
                // Set rwxrwxrwx
                entry.ExternalFileAttributes = 0x1ff << 16;
            }

            archive.PutNextEntry(entry);

            if (entry.IsFile)
            {
                var buf = new byte[4096];

                // Add the file
                using (var fs = File.OpenRead(itemToAdd.FullName))
                {
                    StreamUtils.Copy(fs, archive, buf);
                }
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