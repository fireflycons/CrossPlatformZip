﻿namespace Firefly.CrossPlatformZip.FileSystem
{
    using System;
    using System.IO;

    /// <summary>
    /// Extensions to <see cref="FileSystemInfo"/> to compute relative paths.
    /// </summary>
    internal static class FileSystemInfoExtensions
    {
        /// <summary>
        /// Gets the relative path from given file system object to this one.
        /// </summary>
        /// <param name="to">This <see cref="FileSystemInfo"/>.</param>
        /// <param name="from">A <see cref="FileSystemInfo"/> to compute the relative path from.</param>
        /// <returns>Relative path between the two locations.</returns>
        public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
        {
            return from.GetRelativePathTo(to);
        }

        /// <summary>
        /// Gets the relative path to given file system object from this one.
        /// </summary>
        /// <param name="from">his <see cref="FileSystemInfo"/>.</param>
        /// <param name="to">A <see cref="FileSystemInfo"/> to compute the relative path to.</param>
        /// <returns>Relative path between the two locations.</returns>
        public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
        {
            var fromPath = GetPath(from);
            var toPath = GetPath(to);

            var fromUri = new Uri($"file://{fromPath}");
            var toUri = new Uri($"file://{toPath}");

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Gets the POSIX attributes for this <see cref="FileSystemInfo"/>.
        /// </summary>
        /// <param name="self">This instance.</param>
        /// <returns>A <see cref="PosixAttributes"/> object, which on Windows will be <see cref="PosixAttributes.AllForRoot"/></returns>
        public static PosixAttributes GetPosixAttributes(this FileSystemInfo self)
        {
            return new PosixPermissionsParser(self, new PosixAttributesReader()).Parse();
        }

        private static string GetPath(FileSystemInfo fsi)
        {
            return !(fsi is DirectoryInfo d) ? fsi.FullName : d.FullName.TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
        }

    }
}