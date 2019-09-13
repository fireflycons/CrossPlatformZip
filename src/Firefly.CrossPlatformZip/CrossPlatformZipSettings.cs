namespace Firefly.CrossPlatformZip
{
    using System;

    /// <summary>
    /// Settings class for zip methods
    /// </summary>
    public class CrossPlatformZipSettings
    {
        /// <summary>
        /// Gets or sets the name of the alternate file name for the zip directory,
        /// if you have specified the path to a file to Artifacts property
        /// Allows you to rename the file as it is zipped, e.g.
        /// <c>big_web_packed.js -> index.js</c>
        /// </summary>
        /// <value>
        /// The name of the alternate file.
        /// </value>
        public string AlternateFileName { get; set; }

        /// <summary>
        /// Gets or sets what you want to zip. This can be a single file, or a folder.
        /// </summary>
        /// <value>
        /// The artifact(s) to zip.
        /// </value>
        public string Artifacts { get; set; }

        /// <summary>
        /// Gets or sets the compression level.
        /// </summary>
        /// <value>
        /// The compression level (0 = store, 9 = best).
        /// </value>
        public int CompressionLevel { get; set; } = 9;

        /// <summary>
        /// Gets or sets handler for logging errors (defaults to Console.Error.WriteLine()
        /// </summary>
        /// <value>
        /// The log error.
        /// </value>
        public Action<string> LogError { get; set; } = Console.Error.WriteLine;

        /// <summary>
        /// Gets or sets handler for logging messages (defaults to Console.WriteLine()
        /// </summary>
        /// <value>
        /// The log message.
        /// </value>
        public Action<string> LogMessage { get; set; } = Console.WriteLine;

        /// <summary>
        /// Gets or sets the operating system platform on which you expect the zip to be unzipped.
        /// </summary>
        /// <value>
        /// The target platform, which is one of
        /// <c>ZipPlatform.Windows</c> or <c>ZipPlatform.Unix</c>
        /// ...where the latter is any unix-like OS, (Linux, MacOS etc)
        /// </value>
        public ZipPlatform TargetPlatform { get; set; }

        /// <summary>
        /// Gets or sets the path to the zip file to create.
        /// </summary>
        /// <value>
        /// The zip file.
        /// </value>
        public string ZipFile { get; set; }
    }
}