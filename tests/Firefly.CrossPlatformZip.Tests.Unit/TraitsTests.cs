// ReSharper disable InconsistentNaming
namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Firefly.CrossPlatformZip.PlatformTraits;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Tests for traits classes
    /// </summary>
    public class TraitsTests
    {
        /// <summary>
        /// A file list having two directory names differing only by case
        /// </summary>
        private readonly List<FileSystemInfo> duplicateDirectory =
            new List<FileSystemInfo>
                {
                    new DirectoryInfo("dir1"),
                    new DirectoryInfo("Dir1"),
                    new DirectoryInfo("dir2")
                };

        /// <summary>
        /// A file list having two file names differing only by case
        /// </summary>
        private readonly List<FileSystemInfo> duplicateFile =
            new List<FileSystemInfo>
                {
                    new FileInfo("file1.txt"),
                    new FileInfo("file2.txt"),
                    new FileInfo("File1.txt"),
                };

        /// <summary>
        /// A file list having directory and file name case conflicts
        /// </summary>
        private readonly List<FileSystemInfo> duplicateFilesAndDirectories =
            new List<FileSystemInfo>
                {
                    new FileInfo("file1.txt"),
                    new FileInfo("file2.txt"),
                    new FileInfo("File1.txt"),
                    new DirectoryInfo("dir1"),
                    new DirectoryInfo("Dir1"),
                    new DirectoryInfo("dir2")
                };

        /// <summary>
        /// A file list having no case conflicts
        /// </summary>
        private readonly List<FileSystemInfo> noDuplicates =
            new List<FileSystemInfo>
                {
                    new FileInfo("file1.txt"),
                    new FileInfo("file2.txt"),
                    new DirectoryInfo("dir1"),
                    new DirectoryInfo("dir2")
                };

        /// <summary>
        /// Assert that directories with names differing only in case does not throw when the target is not Windows
        /// </summary>
        [Fact]
        public void GivenPosixTraits_AndCaseSensitiveFileSystemWithDuplicateDirectories_ShouldNotThrow()
        {
            var traits = new PosixPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.duplicateDirectory); });

            action.Should().NotThrow<DuplicateEntryException>();
        }

        /// <summary>
        /// Assert that files with names differing only in case does not throw when the target is not Windows
        /// </summary>
        [Fact]
        public void GivenPosixTraits_AndCaseSensitiveFileSystemWithDuplicateFiles_ShouldNotThrow()
        {
            var traits = new PosixPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.duplicateFile); });

            action.Should().NotThrow<DuplicateEntryException>();
        }

        /// <summary>
        /// Assert that duplicate files and directories does not throw when the target is not Windows
        /// </summary>
        [Fact]
        public void GivenPosixTraits_AndCaseSensitiveFileSystemWithDuplicateFilesAndDirectories_ShouldNotThrow()
        {
            var traits = new PosixPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.duplicateFilesAndDirectories); });

            action.Should().NotThrow<DuplicateEntryException>();
        }

        /// <summary>
        /// Assert that files with no duplicate names does not throw when the target is not Windows
        /// </summary>
        [Fact]
        public void GivenPosixTraits_AndCaseSensitiveFileSystemWithNoDuplicates_ShouldNotThrow()
        {
            var traits = new PosixPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.noDuplicates); });

            action.Should().NotThrow<DuplicateEntryException>();
        }

        /// <summary>
        /// Assert that directories with names differing only in case throws when the target is Windows
        /// </summary>
        [Fact]
        public void GivenWindowsTraits_AndCaseSensitiveFileSystemWithDuplicateDirectories_ShouldThrow()
        {
            var traits = new WindowsPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.duplicateDirectory); });

            action.Should().ThrowExactly<DuplicateEntryException>().And.DuplicateDirectories.Count.Should().Be(2);
        }

        /// <summary>
        /// Assert that files with names differing only in case throws when the target is Windows
        /// </summary>
        [Fact]
        public void GivenWindowsTraits_AndCaseSensitiveFileSystemWithDuplicateFiles_ShouldThrow()
        {
            var traits = new WindowsPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.duplicateFile); });

            action.Should().ThrowExactly<DuplicateEntryException>().And.DuplicateFiles.Count.Should().Be(2);
        }

        /// <summary>
        /// Assert that duplicate files and directories throws when the target is Windows
        /// </summary>
        [Fact]
        public void GivenWindowsTraits_AndCaseSensitiveFileSystemWithDuplicateFilesAndDirectories_ShouldThrow()
        {
            var traits = new WindowsPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.duplicateFilesAndDirectories); });

            action.Should().ThrowExactly<DuplicateEntryException>().And.DuplicateDirectories.Count.Should().Be(2);
        }

        /// <summary>
        /// Assert that files with no duplicate names does not throw when the target is Windows
        /// </summary>
        [Fact]
        public void GivenWindowsTraits_AndCaseSensitiveFileSystemWithNoDuplicates_ShouldNotThrow()
        {
            var traits = new WindowsPlatformTraits();

            var action = new Action(() => { traits.PreValidateFileList(this.noDuplicates); });

            action.Should().NotThrow<DuplicateEntryException>();
        }
    }
}