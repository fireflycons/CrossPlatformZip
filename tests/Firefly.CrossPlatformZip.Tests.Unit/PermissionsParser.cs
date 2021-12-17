namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;
    using System.IO;

    using Firefly.CrossPlatformZip.FileSystem;

    using FluentAssertions;

    using Moq;

    using Xunit;

    /// <summary>
    /// Test parsing of 'ls' output for file attribute and ownership gathering.
    /// </summary>
    public class PermissionsParser
    {
        /// <summary>
        /// Should correctly parse ls output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="expectedOctalAttributes">The expected octal attributes.</param>
        /// <param name="expectedUid">The expected UID.</param>
        /// <param name="expectedGid">The expected GID.</param>
        [Theory]
        [InlineData("-rw-------. 1 0 0 1360 Dec 30  2020 test.txt", "600", 0, 0)]
        [InlineData("-rwx------. 1 0 0 1360 Dec 30  2020 test.txt", "700", 0, 0)]
        [InlineData("---------x. 1 0 0 1360 Dec 30  2020 test.txt", "001", 0, 0)]
        [InlineData("d--------x. 1 0 0 1360 Dec 30  2020 test.txt", "001", 0, 0)]
        [InlineData("-------rwx. 1 0 0 1360 Dec 30  2020 test.txt", "007", 0, 0)]
        [InlineData("-rw-r--r--. 1 0 0 1360 Dec 30  2020 test.txt", "644", 0, 0)]
        [InlineData("-rw-r--r--. 1 1000 2000 1360 Dec 30  2020 test.txt", "644", 1000, 2000)]
        public void ShouldCorrectlyParseLsOutput(
            string output,
            string expectedOctalAttributes,
            int expectedUid,
            int expectedGid)
        {
            var attributeReader = new Mock<IPosixAttributesReader>();

            attributeReader.Setup(r => r.InvokeLs(It.IsAny<FileSystemInfo>())).Returns(output);
            
            var expectedPerms = new PosixAttributes
                                    {
                                        Attributes = Convert.ToInt32(expectedOctalAttributes, 8),
                                        Uid = expectedUid,
                                        Gid = expectedGid
                                    };

            var perms = new PosixPermissionsParser(new FileInfo("test.txt"), attributeReader.Object).Parse();

            perms.Should().BeEquivalentTo(expectedPerms);
        }
    }
}