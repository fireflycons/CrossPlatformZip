namespace Firefly.CrossPlatformZip.PlatformTraits
{
    /// <summary>
    ///     Factory to create the right sort of attribute generator
    /// </summary>
    internal static class PlatformTraitsGeneratorFactory
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
        public static IPlatformTraits GetPlatformTraits(ZipPlatform targetPlatform)
        {
            if (targetPlatform == ZipPlatform.Unix)
            {
                return new PosixPlatformTraits();
            }

            return new WindowsPlatformTraits();
        }
    }
}