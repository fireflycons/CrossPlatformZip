namespace Firefly.CrossPlatformZip
{
    /// <summary>
    /// Target platform for operations.
    /// When creating a zip, central directory will be formatted according to this.
    /// When unzipping, output paths will be forced to the filesystem style according to this
    /// </summary>
    public enum ZipPlatform
    {
        /// <summary>
        /// Platform is Windows
        /// </summary>
        Windows,

        /// <summary>
        /// Platform is Unix/Linux/MacOS
        /// </summary>
        Unix
    }
}
