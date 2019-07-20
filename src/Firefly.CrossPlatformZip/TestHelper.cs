namespace Firefly.CrossPlatformZip
{
    using System.IO;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Helper class to supply paths to files to zip
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// Gets the directory containing this project.
        /// </summary>
        /// <returns>A directory to test zip on.</returns>
        public static string GetZipModuleSourceDirectory()
        {
            return GetCallerPath(true);
        }

        /// <summary>
        /// Gets the path to this file.
        /// </summary>
        /// <returns>A file to test zip on.</returns>
        public static string GetZipModuleSourceFile()
        {
            return GetCallerPath(false);
        }

        /// <summary>
        /// Gets the caller path.
        /// </summary>
        /// <param name="directory">if set to <c>true</c> return the directory; else path to this file.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>The path to the requested type of object</returns>
        private static string GetCallerPath(bool directory, [CallerFilePath] string filePath = "")
        {
            return directory ? Path.GetDirectoryName(filePath) : filePath;
        }
    }
}