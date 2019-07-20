﻿// ReSharper disable InconsistentNaming

namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;

    using Firefly.CrossPlatformZip;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for zipping files
    /// </summary>
    [TestClass]
    public class ZipTests
    {
        /// <summary>
        /// Given a directory, assert that all files and directories within are added with unix paths.
        /// </summary>
        [TestMethod]
        public void
            GivenADirectory_AndWeDontSpecifyTheTargetOperatingSystem_ThenAllPathsWithinZipAreForTheCurrentOperatingSystem()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(zipFile, directoryToZip);

                this.GetAllEntries(zipFile)
                    .All(e => e.Contains(Path.DirectorySeparatorChar) || e.IndexOfAny(new[] { '\\', '/' }) == -1)
                    .Should().BeTrue();
            }
        }

        /// <summary>
        /// Given a directory, assert that all files and directories within are added with unix paths.
        /// </summary>
        [TestMethod]
        public void GivenADirectory_AndWeExplicityWantToCreateUnixArchive_ThenAllPathsWithinZipAreUnix()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(zipFile, directoryToZip, ZipPlatform.Unix);

                this.GetAllEntries(zipFile).Any(e => e.Contains("\\")).Should().BeFalse();
            }
        }

        /// <summary>
        /// Given a directory, assert that all files and directories within are added with unix paths.
        /// </summary>
        [TestMethod]
        public void GivenADirectory_AndWeExplicityWantToCreateWindowsArchive_ThenAllPathsWithinZipAreWindows()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(zipFile, directoryToZip, ZipPlatform.Windows);

                this.GetAllEntries(zipFile).Any(e => e.Contains("/")).Should().BeFalse();
            }
        }

        /// <summary>
        /// Given a directory, assert that all files and directories within are added recursively.
        /// </summary>
        [TestMethod]
        public void GivenADirectory_ThenAllFilesAndDirectoriesWithinAreAddedToZip()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();
            var expectedEntryCount =
                Directory.EnumerateFileSystemEntries(directoryToZip, "*", SearchOption.AllDirectories).Count();

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(zipFile, directoryToZip);

                this.GetEntryCount(zipFile).Should().Be(expectedEntryCount);
            }
        }

        /// <summary>
        /// Given a single file and using <see cref="Zipper.Zip(string,string)"/> method then it is added at the root of the central directory.
        /// </summary>
        [TestMethod]
        public void GivenASingleFile_AndUsingZipMethod_ThenItIsAddedAtTheRootOfTheCentralDirectory()
        {
            var fileToZip = TestHelper.GetZipModuleSourceFile();
            var expectedEntry = Path.GetFileName(fileToZip);

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.Zip(zipFile, fileToZip, ZipPlatform.Unix);

                this.GetEntryCount(zipFile).Should().Be(1);
                this.GetFirstEntryPath(zipFile).Should().Be(expectedEntry);
            }
        }

        /// <summary>
        /// Given a single file and using <see cref="Zipper.ZipSingleFile(string,string,string)"/> method with the alternate name argument = <c>null</c>
        /// then it is added at the root of the central directory.
        /// </summary>
        [TestMethod]
        public void GivenASingleFile_AndUsingZipSingleFileMethod_ThenItIsAddedAtTheRootOfTheCentralDirectory()
        {
            var fileToZip = TestHelper.GetZipModuleSourceFile();
            var expectedEntry = Path.GetFileName(fileToZip);

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.ZipSingleFile(zipFile, fileToZip, null);

                this.GetEntryCount(zipFile).Should().Be(1);
                this.GetFirstEntryPath(zipFile).Should().Be(expectedEntry);
            }
        }

        /// <summary>
        /// Given a single file and using <see cref="Zipper.ZipSingleFile(string,string,string)"/> method with the alternate name argument not <c>null</c>
        /// then it is added at the root of the central directory with the alternate filename.
        /// </summary>
        [TestMethod]
        public void
            GivenASingleFile_AndUsingZipSingleFileMethodWithAlternateName_ThenItIsAddedWithAlternateNameAtTheRootOfTheCentralDirectory()
        {
            var fileToZip = TestHelper.GetZipModuleSourceFile();
            var expectedEntry = "index.js";

            using (var zipFile = new TempFile("test.zip"))
            {
                Zipper.ZipSingleFile(zipFile, fileToZip, expectedEntry);

                this.GetEntryCount(zipFile).Should().Be(1);
                this.GetFirstEntryPath(zipFile).Should().Be(expectedEntry);
            }
        }

        private IEnumerable<string> GetAllEntries(string zipFile)
        {
            using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Read))
            {
                return archive.Entries.Select(e => e.FullName).ToList();
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