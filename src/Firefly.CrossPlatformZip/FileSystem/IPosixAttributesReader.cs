namespace Firefly.CrossPlatformZip.FileSystem
{
    using System.IO;

    /// <summary>
    /// Interface to mechanism to execute 'ls' on a file to retrieve POSIX information.
    /// NET standard currently cannot retrieve this data
    /// </summary>
    internal interface IPosixAttributesReader
    {
        /// <summary>
        /// Invokes ls on the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Single line output of ls command</returns>
        string InvokeLs(FileSystemInfo file);
    }
}