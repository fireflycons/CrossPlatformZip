namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;
    using System.IO;

    using Firefly.CrossPlatformZip.FileSystem;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Tests linux executable detection
    /// </summary>
    public class ExecutableFile
    {
        /// <summary>
        /// Should detect executable from ELF file header.
        /// </summary>
        [Fact]
        public void ShouldDetectELFBinary()
        {
            // Enough bytes from the start of an ELF binary for the detection to work
            var elfBinaryData = new byte[]
                                    {
                                        0x7f, 0x45, 0x4c, 0x46, 0x02, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x02, 0x00, 0x3e, 0x00, 0x01, 0x00, 0x00, 0x00, 0x50, 0x04,
                                        0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0xe0, 0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x40, 0x00, 0x38, 0x00, 0x09, 0x00, 0x40, 0x00, 0x1c, 0x00, 0x1b, 0x00
                                    };

            using (var elfBinary = new TempFile($"elf-{Guid.NewGuid()}"))
            {
                using (var s = File.Create(elfBinary))
                {
                    s.Write(elfBinaryData);
                }

                new ExecutableFileDetector().IsExecutable(new FileInfo(elfBinary)).Should().BeTrue();
            }
        }

        /// <summary>
        /// Should detect script with shebang
        /// </summary>
        [Fact]
        public void ShouldDetectShellScript()
        {
            using (var scriptFile = new TempFile($"script-{Guid.NewGuid()}"))
            {
                File.WriteAllText(scriptFile, "#!/usr/bin/bash\necho \"hi\"");

                new ExecutableFileDetector().IsExecutable(new FileInfo(scriptFile)).Should().BeTrue();
            }
        }
    }
}