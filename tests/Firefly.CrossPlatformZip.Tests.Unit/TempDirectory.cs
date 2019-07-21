namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;
    using System.IO;

    /// <summary>
    ///     Helper class to generate disposable temporary directories
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TempDirectory : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TempDirectory" /> class.
        /// </summary>
        public TempDirectory()
        {
            this.FullName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(this.FullName);
        }

        /// <summary>
        ///     Gets the full name of the temp file.
        /// </summary>
        /// <value>
        ///     The full name.
        /// </value>
        public string FullName { get; }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="TempFile" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="tempDirectory">The temporary directory.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator string(TempDirectory tempDirectory)
        {
            return tempDirectory.FullName;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Directory.Exists(this.FullName))
            {
                Directory.Delete(this.FullName, true);
            }
        }
    }
}