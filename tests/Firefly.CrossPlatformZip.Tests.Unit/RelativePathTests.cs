// ReSharper disable InconsistentNaming

namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System.IO;

    using Firefly.CrossPlatformZip;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Test relative path extension method
    /// </summary>
    public class RelativePathTests
    {
        /// <summary>
        /// Given the path to a directory and path to file beneath this directory then correct relative path from is generated.
        /// </summary>
        [Fact]
        public void GivenPathToDirectoryAndPathToFileBeneathThisDirectory_ThenCorrectRelativePathFromIsGenerated()
        {
            // Build paths with Path.Combine so tests work in Windows or Unix
            var dir = new DirectoryInfo(Path.DirectorySeparatorChar + "temp");
            var file = new FileInfo(Path.DirectorySeparatorChar + Path.Combine("temp", "dir", "file.txt"));
            var expected = Path.Combine("dir", "file.txt");

            var relative = file.GetRelativePathFrom(dir);

            relative.Should().Be(expected);
        }

        /// <summary>
        /// Given the path to a directory and path to file not beneath this directory then correct relative path to is generated.
        /// </summary>
        [Fact]
        public void GivenPathToDirectoryAndPathToFileNotBeneathThisDirectory_ThenCorrectRelativePathFromIsGenerated()
        {
            var dir = new DirectoryInfo(Path.DirectorySeparatorChar + "temp");
            var file = new FileInfo(Path.DirectorySeparatorChar + Path.Combine("not-temp", "dir", "file.txt"));
            var expected = Path.Combine("..", "not-temp", "dir", "file.txt");

            var relative = file.GetRelativePathFrom(dir);

            relative.Should().Be(expected);
        }

        /// <summary>
        /// Given the path to a directory and path to file beneath this directory then correct relative path to is generated.
        /// </summary>
        [Fact]
        public void GivenPathToDirectoryAndPathToFileBeneathThisDirectory_ThenCorrectRelativePathToIsGenerated()
        {
            // Build paths with Path.Combine so tests work in Windows or Unix
            var dir = new DirectoryInfo(Path.DirectorySeparatorChar + "temp");
            var file = new FileInfo(Path.DirectorySeparatorChar + Path.Combine("temp", "dir", "file.txt"));
            var expected = Path.Combine("dir", "file.txt");

            var relative = dir.GetRelativePathTo(file);

            relative.Should().Be(expected);
        }

        /// <summary>
        /// Given the path to a directory and path to file not beneath this directory then correct relative path to is generated.
        /// </summary>
        [Fact]
        public void GivenPathToDirectoryAndPathToFileNotBeneathThisDirectory_ThenCorrectRelativePathToIsGenerated()
        {
            var dir = new DirectoryInfo(Path.DirectorySeparatorChar + "temp");
            var file = new FileInfo(Path.DirectorySeparatorChar + Path.Combine("not-temp", "dir", "file.txt"));
            var expected = Path.Combine("..", "not-temp", "dir", "file.txt");

            var relative = dir.GetRelativePathTo(file);

            relative.Should().Be(expected);
        }

        /// <summary>
        /// Relative path from and relative path to should gave same result when arguments reversed in the To call
        /// </summary>
        [Fact]
        public void RelativePathFrom_AndRelativePathTo_ShouldGenerateSamePathWhenArgumentsReversed()
        {
            var dir = new DirectoryInfo(Path.DirectorySeparatorChar + "temp");
            var file = new FileInfo(Path.DirectorySeparatorChar + Path.Combine("not-temp", "dir", "file.txt"));

            file.GetRelativePathFrom(dir).Should().Be(dir.GetRelativePathTo(file));
        }
    }
}