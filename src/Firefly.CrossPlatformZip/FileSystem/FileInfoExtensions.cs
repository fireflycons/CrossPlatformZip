namespace Firefly.CrossPlatformZip.FileSystem
{
    using System.IO;

    internal static class FileInfoExtensions
    {
        public static bool IsExecutable(this FileInfo self)
        {
            return new ExecutableFileDetector().IsExecutable(self);
        }
    }
}
