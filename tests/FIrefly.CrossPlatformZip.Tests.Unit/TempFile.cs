namespace Firefly.CrossPlatformZip.Tests.Unit
{
    using System;
    using System.IO;

    /// <summary>
    /// Helper class to generate disposable temporary files
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TempFile : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempFile"/> class.
        /// </summary>
        /// <param name="name">The file name.</param>
        public TempFile(string name)
        {
            this.FullName = Path.Combine(Path.GetTempPath(), name);
        }

        /// <summary>
        /// Gets the full name of the temp file.
        /// </summary>
        /// <value>
        /// The full name.
        /// </value>
        public string FullName { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TempFile"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="tempFile">The temporary file.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(TempFile tempFile)
        {
            return tempFile.FullName;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (File.Exists(this.FullName))
            {
                File.Delete(this.FullName);
            }
        }
    }
}