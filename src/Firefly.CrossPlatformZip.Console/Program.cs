namespace Firefly.CrossPlatformZip.Console
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Very simple interface to create a new zip file. 
        /// </summary>
        /// <param name="zipFile">Path to zip file to create</param>
        /// <param name="platform">Target platform (default Unix if running on Windows, else Windows).</param>
        /// <param name="level">Compression level (0-9, 0 = store, 9 = best, default 5)</param>
        /// <param name="argument">Path to file or folder to zip</param>
        /// <returns>Exit status</returns>
        public static int Main(string zipFile, ZipPlatform? platform, int? level, FileSystemInfo argument)
        {
            try
            {
                var compession = level ?? 5;
                var defaultPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                          ? ZipPlatform.Unix
                                          : ZipPlatform.Windows;

                Zipper.Zip(
                    new CrossPlatformZipSettings
                        {
                            CompressionLevel = compession, Artifacts = argument.FullName, ZipFile = zipFile, TargetPlatform = platform ?? defaultPlatform
                    });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }

            return 0;
        }
    }
}