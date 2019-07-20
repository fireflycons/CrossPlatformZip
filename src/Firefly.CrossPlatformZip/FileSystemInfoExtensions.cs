namespace Firefly.CrossPlatformZip
{
    using System;
    using System.IO;

    /// <summary>
    /// Extensions to <see cref="FileSystemInfo"/> to compute relative paths.
    /// </summary>
    internal static class FileSystemInfoExtensions
    {
        public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
        {
            return from.GetRelativePathTo(to);
        }

        public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
        {
            var fromPath = GetPath(from);
            var toPath = GetPath(to);

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath.Replace('/', Path.DirectorySeparatorChar);
        }
        private static string GetPath(FileSystemInfo fsi)
        {
            var d = fsi as DirectoryInfo;
            return d == null ? fsi.FullName : d.FullName.TrimEnd('\\', '/') + "\\";
        }

    }
}