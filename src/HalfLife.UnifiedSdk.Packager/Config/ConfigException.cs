namespace HalfLife.UnifiedSdk.Packager.Config
{
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
