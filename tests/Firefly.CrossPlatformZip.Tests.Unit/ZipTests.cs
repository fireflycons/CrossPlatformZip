// ReSharper disable InconsistentNaming

namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using Firefly.CrossPlatformZip;

    using FluentAssertions;

    using Xunit;

    /// <summary>
    /// Tests for zipping files
    /// </summary>
    public class ZipTests
    {
        /// <summary>
        /// Given a directory, assert that all files and directories within are added with unix paths.
        /// </summary>
        [Fact]
        public void GivenADirectory_AndWeExplicityWantToCreateUnixArchive_ThenAllPathsWithinZipAreUnix()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(
                    new CrossPlatformZipSettings
                        {
                            ZipFile = zipFile,
                            Artifacts = directoryToZip,
                            CompressionLevel = 9,
                            TargetPlatform = ZipPlatform.Unix
                        });

                this.GetAllEntries(zipFile).Any(e => e.Contains("\\")).Should().BeFalse();
            }
        }

        /// <summary>
        /// Given a directory, assert that all files and directories within are added with unix paths.
        /// </summary>
        [Fact]
        public void GivenADirectory_AndWeExplicityWantToCreateWIndowsArchive_ThenAllPathsWithinZipAreWindows()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(
                    new CrossPlatformZipSettings
                        {
                            ZipFile = zipFile,
                            Artifacts = directoryToZip,
                            CompressionLevel = 9,
                            TargetPlatform = ZipPlatform.Windows
                        });


                // this.GetAllEntries(zipFile).Any(e => e.Contains("\\")).Should().BeFalse();
            }
        }

        /// <summary>
        /// Given a directory, assert that all files and directories within are added recursively.
        /// </summary>
        [Fact]
        public void GivenADirectory_ThenAllFilesAndDirectoriesWithinAreAddedToZip()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();
            var expectedEntryCount =
                Directory.EnumerateFileSystemEntries(directoryToZip, "*", SearchOption.AllDirectories).Count();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(
                    new CrossPlatformZipSettings
                        {
                            ZipFile = zipFile,
                            Artifacts = directoryToZip,
                            CompressionLevel = 9,
                        });


                this.GetEntryCount(zipFile).Should().Be(expectedEntryCount);
            }
        }

        /// <summary>
        /// Given a single file and using <see cref="Zipper.Zip(CrossPlatformZipSettings)"/> method then it is added at the root of the central directory.
        /// </summary>
        [Fact]
        public void GivenASingleFile_AndUsingZipMethod_ThenItIsAddedAtTheRootOfTheCentralDirectory()
        {
            var fileToZip = TestHelper.GetZipModuleSourceFile();
            var expectedEntry = Path.GetFileName(fileToZip);

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(
                    new CrossPlatformZipSettings
                        {
                            ZipFile = zipFile,
                            Artifacts = fileToZip,
                            CompressionLevel = 9,
                            TargetPlatform = ZipPlatform.Unix
                        });


                this.GetEntryCount(zipFile).Should().Be(1);
                this.GetFirstEntryPath(zipFile).Should().Be(expectedEntry);
            }
        }

        /// <summary>
        /// Given a single file and using <see cref="Zipper.ZipSingleFile(CrossPlatformZipSettings)"/> method with the alternate name argument = <c>null</c>
        /// then it is added at the root of the central directory.
        /// </summary>
        [Fact]
        public void GivenASingleFile_AndUsingZipSingleFileMethod_ThenItIsAddedAtTheRootOfTheCentralDirectory()
        {
            var fileToZip = TestHelper.GetZipModuleSourceFile();
            var expectedEntry = Path.GetFileName(fileToZip);

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.ZipSingleFile(
                    new CrossPlatformZipSettings
                        {
                            ZipFile = zipFile,
                            Artifacts = fileToZip,
                            CompressionLevel = 9,
                            TargetPlatform = ZipPlatform.Unix
                        });


                this.GetEntryCount(zipFile).Should().Be(1);
                this.GetFirstEntryPath(zipFile).Should().Be(expectedEntry);
            }
        }

        /// <summary>
        /// Given a single file and using <see cref="Zipper.ZipSingleFile(CrossPlatformZipSettings)"/> method with the alternate name argument not <c>null</c>
        /// then it is added at the root of the central directory with the alternate filename.
        /// </summary>
        [Fact]
        public void
            GivenASingleFile_AndUsingZipSingleFileMethodWithAlternateName_ThenItIsAddedWithAlternateNameAtTheRootOfTheCentralDirectory()
        {
            var fileToZip = TestHelper.GetZipModuleSourceFile();
            var expectedEntry = "index.js";

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.ZipSingleFile(
                    new CrossPlatformZipSettings
                        {
                            ZipFile = zipFile,
                            Artifacts = fileToZip,
                            CompressionLevel = 9,
                            AlternateFileName = expectedEntry
                        });

                this.GetEntryCount(zipFile).Should().Be(1);
                this.GetFirstEntryPath(zipFile).Should().Be(expectedEntry);
            }
        }

        /// <summary>
        /// Gets all entries from zip central directory.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <returns>Number of entries in central directory.</returns>
        private IEnumerable<string> GetAllEntries(string zipFile)
        {
            using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Read))
            {
                var entryNames = archive.Entries.Select(e => e.FullName).ToList();
                return entryNames;
            }
        }

        /// <summary>
        /// Gets the zip file entry count.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <returns>Number of items in the zip directory.</returns>
        private int GetEntryCount(string zipFile)
        {
            using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Read))
            {
                return archive.Entries.Count;
            }
        }

        /// <summary>
        /// Gets the path of the first entry in the given zip.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <returns>Full path of first entry.</returns>
        private string GetFirstEntryPath(string zipFile)
        {
            using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Read))
            {
                return archive.Entries.First().FullName;
            }
        }
    }
}