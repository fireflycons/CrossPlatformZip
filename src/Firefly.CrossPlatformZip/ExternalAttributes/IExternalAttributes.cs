// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExternalAttributes.cs" company="">
//   
// </copyright>
// <summary>
//   Describes mechanism for generating ZIP external attributes field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Firefly.CrossPlatformZip.ExternalAttributes
{
    using System.IO;

    /// <summary>
    ///     Describes mechanism for generating ZIP external attributes field.
    /// </summary>
    internal interface IExternalAttributes
    {
        /// <summary>
        ///     Gets the platform-specific directory separator.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        char DirectorySeparator { get; }

        /// <summary>
        ///     Gets the directory separator for foreign OS, e.g Windows separator on Posix and vice-versa.
        /// </summary>
        /// <value>
        ///     The directory separator.
        /// </value>
        char ForeignDirectorySeparator { get; }

        /// <summary>
        /// Gets the ZIP external attributes for the given file system object.
        /// </summary>
        /// <param name="fileSystemObject">
        /// The file system object.
        /// </param>
        /// <returns>
        /// ZIP external attribute
        /// </returns>
        int GetExternalAttributes(FileSystemInfo fileSystemObject);

        /// <summary>
        /// Gets the extra data records (if any) to add to new entry.
        /// </summary>
        /// <param name="fileSystemObject">The file system object.</param>
        /// <returns>Byte array of raw extra data.</returns>
        byte[] GetExtraDataRecords(FileSystemInfo fileSystemObject);
    }
}