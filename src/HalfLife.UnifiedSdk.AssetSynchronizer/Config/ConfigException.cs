namespace HalfLife.UnifiedSdk.AssetSynchronizer.Config
{
    // TODO: should refactor packager and this to use common code
    internal sealed class ConfigException : Exception
    {
        public ConfigException()
        {
        }

        public ConfigException(string? message)
            : base(message)
        {
        }

        public ConfigException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
