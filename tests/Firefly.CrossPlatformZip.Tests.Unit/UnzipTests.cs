

// ReSharper disable InconsistentNaming
namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unzip tests
    /// </summary>
    [TestClass]
    public class UnzipTests
    {
        /// <summary>
        /// What OS is the running on?
        /// </summary>
        private static readonly bool IsWindows = Path.DirectorySeparatorChar == '\\';

        /// <summary>
        /// Test that if a zip is created for a foreign platform e.g. Windows and unzipped on Linux (and vice versa depending on where test is run)
        /// then file files are extracted correctly with no corruption.
        /// </summary>
        [TestMethod]
        public void GivenZipCreatedForForeignPlatform_ThenFilesExtractedCorrectlyAndAreNotCorrupted()
        {
            var directoryToZip = TestHelper.GetZipModuleSourceDirectory();

            var numItems = Directory.EnumerateFileSystemEntries(directoryToZip, "*", SearchOption.AllDirectories)
                .Count();
            var numDirectories = Directory.EnumerateDirectories(directoryToZip, "*", SearchOption.AllDirectories)
                .Count();
            var inputFiles = Directory.EnumerateFiles(directoryToZip, "*", SearchOption.AllDirectories).ToList();

            using (var zipFile = new TempFile("test.zip"))
            using (var tempDir = new TempDirectory())
            {
                // Generate zip for foreign platform
                Zipper.Zip(zipFile, directoryToZip, IsWindows ? ZipPlatform.Unix : ZipPlatform.Windows);

                // Now unzip it
                // Some unzips, especially on Unix extracting zips with Windows paths get it wrong, creating files like 'dir/dir2/file.txt' rather than directory structure
                Zipper.Unzip(zipFile, tempDir);

                var extractedItems = Directory.EnumerateFileSystemEntries(tempDir, "*", SearchOption.AllDirectories)
                    .Count();
                var extractedirectories =
                    Directory.EnumerateDirectories(tempDir, "*", SearchOption.AllDirectories).Count();
                var extratedFiles = Directory.EnumerateFiles(tempDir, "*", SearchOption.AllDirectories).ToList();

                numItems.Should().Be(extractedItems, "total number of extracted items should be the same");
                numItems.Should().Be(extractedItems, "total number of extracted items should be the same");
                numDirectories.Should().Be(
                    extractedirectories,
                    "total number of extracted directories should be the same - Unix may extract windows directories as files with / in name");
                inputFiles.Count.Should().Be(extratedFiles.Count, "total number of extracted files should be the same");

                // Check file hashes to to assert no corruption.
                var inputFilesToCheck = inputFiles.ToDictionary(f => f.Substring(directoryToZip.Length), f => f);
                var outputFilesToCheck = extratedFiles.ToDictionary(f => f.Substring(tempDir.FullName.Length), f => f);

                foreach (var kv in inputFilesToCheck)
                {
                    var outputFile = outputFilesToCheck[kv.Key];
                    Md5Hash(kv.Value).Should().Be(Md5Hash(outputFile), "file should not be corrupted.");
                }
            }
        }

        /// <summary>
        /// Compute MD5 hash of file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>MD5 hash of string.</returns>
        private static string Md5Hash(string path)
        {
            using (var fs = File.OpenRead(path))
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(fs));
            }
        }
    }
}