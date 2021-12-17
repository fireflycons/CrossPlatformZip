namespace Firefly.CrossPlatformZip.FileSystem
{
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Detects files that should be marked as executable when targeting Unix from a Windows system
    /// </summary>
    internal class ExecutableFileDetector
    {
        /// <summary>
        /// The ELF header (0x7F followed by ELF(45 4c 46) in ASCII)
        /// </summary>
        private static readonly byte[] ElfHeader = { 0x7f, 0x45, 0x4c, 0x46 };

        /// <summary>
        /// Regex to match shebang
        /// </summary>
        private static readonly Regex ShebangRegex = new Regex(@"^#!(/\w+)+");

        /// <summary>
        /// Determines whether the specified file is executable.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   <c>true</c> if the specified file is executable; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExecutable(FileInfo file)
        {
            if (!file.Exists || file.Length < 4)
            {
                return false;
            }

            return IsELFBinary(file) || IsExecutableScript(file);
        }

        /// <summary>
        /// Determines whether the specified file is an ELF binary.
        /// </summary>
        /// <param name="fileInfo">The file.</param>
        /// <returns>
        ///   <c>true</c> if the file is an ELF binary; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso href="https://en.wikipedia.org/wiki/Executable_and_Linkable_Format#File_header"/>
        // ReSharper disable once InconsistentNaming
        private static bool IsELFBinary(FileInfo fileInfo)
        {
            if (fileInfo.Length < 52)
            {
                // Less than min length of ELF header.
                return false;
            }

            // Check it has an ELF header and the following fields are within range
            using (var s = fileInfo.OpenRead())
            {
                var header = new byte[4];
                s.Read(header, 0, 4);

                if (!ElfHeader.SequenceEqual(header))
                {
                    return false;
                }

                // EI_CLASS
                var cls = s.ReadByte();

                if (cls < 1 || cls > 2)
                {
                    return false;
                }

                // EI_DATA
                var endian = s.ReadByte();

                if (endian < 1 || endian > 2)
                {
                    return false;
                }

                // EI_VERSION
                if (s.ReadByte() != 1)
                {
                    return false;
                }

                // EI_OSABI
                if (s.ReadByte() > 0x12)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified file is an executable script by looking for a shebang.
        /// </summary>
        /// <param name="fileInfo">The file.</param>
        /// <returns>
        ///   <c>true</c> if the file is an executable script; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsExecutableScript(FileInfo fileInfo)
        {
            using (var sr = new StreamReader(fileInfo.OpenRead()))
            {
                // ReSharper disable once AssignNullToNotNullAttribute - already checked file is not zero bytes.
                return ShebangRegex.IsMatch(sr.ReadLine());
            }
        }
    }
}