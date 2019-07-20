// ReSharper disable InconsistentNaming

namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System.IO;

    using Firefly.CrossPlatformZip;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test relative path extension method
    /// </summary>
    [TestClass]
    public class RelativePathTests
    {
        /// <summary>
        /// Given the path to a directory and path to file beneath this directory then correct relative path is generated.
        /// </summary>
        [TestMethod]
        public void GivenPathToDirectoryAndPathToFileBeneathThisDirectory_ThenCorrectRelativePathIsGenerated()
        {
            // Build paths with Path.Combine so tests work in Windows or Unix
            var dir = new DirectoryInfo(Path.DirectorySeparatorChar + "temp");
            var file = new FileInfo(Path.DirectorySeparatorChar + Path.Combine("temp", "dir", "file.txt"));
            var expected = Path.Combine("dir", "file.txt");

            var relative = file.GetRelativePathFrom(dir);

            relative.Should().Be(expected);
        }
    }
}