namespace Firefly.CrossPlatformZip.ExternalAttributes
{
    /// <summary>
    ///     Factory to create the right sort of attribute generator
    /// </summary>
    internal static class ExternalAttributeGeneratorFactory
    {
        /// <summary>
        /// Gets the external attributes generator.
        /// </summary>
        /// <param name="targetPlatform">
        /// The target platform.
        /// </param>
        /// <returns>
        /// External attribute generator interface
        /// </returns>
        public static IExternalAttributes GetExternalAttributesGenerator(ZipPlatform targetPlatform)
        {
            if (targetPlatform == ZipPlatform.Unix)
            {
                return new PosixExternalAttributes();
            }

            return new WindowsExternalAttributes();
        }
    }
}