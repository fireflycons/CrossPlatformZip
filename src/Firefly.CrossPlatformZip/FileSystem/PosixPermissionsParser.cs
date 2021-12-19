namespace Firefly.CrossPlatformZip.FileSystem
{
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Parses linux attributes from output of ls command
    /// </summary>
    internal class PosixPermissionsParser
    {
        /// <summary>
        /// Regex to parse output of ls
        /// </summary>
        public static readonly Regex FileListOutputRegex =
            new Regex(@"^[dls\-](?<fileattr>[rwx\-]{9}).?\s*\d+\s+(?<uid>\d+)\s+(?<gid>\d+)");


        /// <summary>
        /// The file to get attributes for
        /// </summary>
        private readonly FileSystemInfo file;

        /// <summary>
        /// The attribute reader implementation
        /// </summary>
        private readonly IPosixAttributesReader attributeReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="PosixPermissionsParser"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="attributeReader">The attribute reader.</param>
        public PosixPermissionsParser(FileSystemInfo file, IPosixAttributesReader attributeReader)
        {
            this.attributeReader = attributeReader;
            this.file = file;
        }

        /// <summary>
        /// Parses the ls output.
        /// </summary>
        /// <returns>A <see cref="PosixAttributes"/> object</returns>
        public PosixAttributes Parse()
        {
            var output = this.attributeReader.InvokeLs(this.file);

            if (string.IsNullOrEmpty(output))
            {
                return PosixAttributes.AllForRoot;
            }

            var m = FileListOutputRegex.Match(output);

            if (!m.Success)
            {
                // Some error - just assume root and all permissions
                return PosixAttributes.AllForRoot;
            }

            var attributes = 0;

            foreach (var c in m.Groups["fileattr"].Value)
            {
                attributes <<= 1;
                attributes |= c == '-' ? 0 : 1;
            }

            return new PosixAttributes
                       {
                           Attributes = attributes,
                           Uid = int.Parse(m.Groups["uid"].Value),
                           Gid = int.Parse(m.Groups["gid"].Value)
                       };
        }
    }
}